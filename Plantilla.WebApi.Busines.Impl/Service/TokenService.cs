using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Plantilla.WebApi.Busines.Contracts.Domain;
using Plantilla.WebApi.Busines.Contracts.Security;
using Plantilla.WebApi.Busines.Contracts.Service;
using Plantilla.WebApi.Data.Contracts.Model;
using Plantilla.WebApi.Data.Contracts.Repository;
using Plantilla.WebApi.Data.Contracts.Repository.OAuth;
using Plantilla.WebApi.Dto;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Plantilla.WebApi.Busines.Impl.Service
{
    public class TokenService : ITokenService
    {
        private readonly JwtTokenConfig _jwtTokenConfig;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRolesRepository;
        private readonly IRoleRepository _rolesRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenFactory _refreshTokenFactory;

        public TokenService(JwtTokenConfig jwtTokenConfig,
                            IUserRepository userRepository,
                            IUserRoleRepository userRolesRepository,
                            IRoleRepository rolesRepository,
                            IRefreshTokenRepository refreshTokenRepository,
                            IConfiguration configuration,
                            IRefreshTokenFactory refreshTokenFactory)
        {
            _jwtTokenConfig = jwtTokenConfig;
            _userRepository = userRepository;
            _userRolesRepository = userRolesRepository;
            _rolesRepository = rolesRepository;
            _refreshTokenRepository = refreshTokenFactory.GetRefreshTokenService();
            _configuration = configuration;
            _refreshTokenFactory = refreshTokenFactory;
        }

        private OperationResult<TokenResponse> GenerateToken(string user, IList<string> roles, Guid jti = default)
        {
            var result = new OperationResult<TokenResponse>();

            var securityDescriptorResult = GenerateSecurityTokenDescriptor(user, roles, _jwtTokenConfig.Issuer, _jwtTokenConfig.Audience, jti);

            if (!securityDescriptorResult.HasErrors())
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                var token = tokenHandler.CreateToken(securityDescriptorResult.Result);

                string jwtToken = tokenHandler.WriteToken(token);

                var expiration = TimeSpan.FromMinutes(_jwtTokenConfig.AccessTokenExpiration).TotalSeconds;

                return new OperationResult<TokenResponse>(new TokenResponse
                {
                    AccessToken = jwtToken,
                    TokenType = _jwtTokenConfig.Type,
                    ExpiresIn = Convert.ToInt64(expiration),
                    RefreshToken = BuildRefreshToken()
                });
            }

            return result;
        }

        public async Task<OperationResult<TokenResponse>> GenerateToken(TokenRequest request)
        {
            var result = new OperationResult<TokenResponse>();

            try
            {
                var user = await _userRepository.GetByUsernameAsync(request.Username);

                if (user != null)
                {
                    if (user.VerifyPassword(request.Password))
                    {
                        var rolesPerUser = await _userRolesRepository.GetByUserIdAsync(user.Id);
                        var roles = await _rolesRepository.GetAllRolesAsync(rolesPerUser.Select(r => r.RoleId));
                        var tokenResponse = GenerateToken(user.UserName, roles.Select(r => r.Name).ToList());

                        var claim = GetPrincipalFromToken(tokenResponse.Result.AccessToken);

                        if (claim != null)
                        {
                            var jti = Guid.Parse(claim.Claims
                                                .Where(c => c.Type == "jti")
                                                .First().Value);

                            var refreshTokenEntity = new RefreshToken
                            {
                                Id = Guid.NewGuid(),
                                Token = tokenResponse.Result.RefreshToken,
                                JTI = jti,
                                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtTokenConfig.RefreshTokenExpiration),
                                IssuedAt = DateTime.UtcNow
                            };

                            //await _refreshTokenRepository.AddAsync(refreshTokenEntity);

                            return tokenResponse;
                        }

                    }
                }
            }
            catch (Exception e)
            {
                result.AddError(e);
                throw;
            }
            return result;
        }


        public async Task<OperationResult<TokenResponse>> RefreshToken(string accessToken, string refreshToken)
        {
            var result = new OperationResult<TokenResponse>();
            var claim = GetPrincipalFromToken(accessToken);

            if (claim != null)
            {
                var jti = Guid.Parse(claim.Claims
                                        .Where(c => c.Type == "jti")
                                        .First().Value);

                var refreshTokenPerse = await _refreshTokenRepository.GetTokenByJti(jti);

                if (refreshTokenPerse == refreshToken)
                {
                    await _refreshTokenRepository.DeleteByJTIAsync(jti);

                    var newRefreshToken = new RefreshToken
                    {
                        Id = Guid.NewGuid(),
                        ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtTokenConfig.RefreshTokenExpiration),
                        IssuedAt = DateTime.UtcNow,
                        JTI = jti,
                        Token = refreshToken
                    };

                    await _refreshTokenRepository.AddAsync(newRefreshToken);

                    result.Result = new TokenResponse
                    {
                        ExpiresIn = 10,
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        TokenType = "Bearer"
                    };

                    return result;
                }
            }
            return null;
        }

        private string BuildRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            JwtSecurityTokenHandler tokenValidator = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenConfig.Secret));

            var parameters = new TokenValidationParameters
            {
                ValidAudience = _jwtTokenConfig.Audience,
                ValidIssuer = _jwtTokenConfig.Issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateLifetime = false
            };

            try
            {
                var principal = tokenValidator.ValidateToken(token, parameters, out var securityToken);

                if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }


                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private OperationResult<SecurityTokenDescriptor> GenerateSecurityTokenDescriptor(string user,
                                                                                             IList<string> roles,
                                                                                             string issuer,
                                                                                             string audience,
                                                                                             Guid jti = default)
        {
            var response = new OperationResult<SecurityTokenDescriptor>();

            try
            {
                var mySecret = _jwtTokenConfig.Secret;

                var tokenExpirationTime = _jwtTokenConfig.AccessTokenExpiration;

                var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                            {
                            new Claim("jti",jti == Guid.Empty ? Guid.NewGuid().ToString() : jti.ToString()), //id de sesión
                            new Claim(ClaimTypes.Name, user), //nombre de usuario 
                            new Claim(ClaimTypes.DateOfBirth, DateTime.Now.Subtract(TimeSpan.FromDays (20 * 365)).ToString("yyyy-MM-dd")),
                            }),
                    NotBefore = DateTime.UtcNow,
                    Expires = DateTime.UtcNow.AddMinutes(tokenExpirationTime),
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature),
                    IssuedAt = DateTime.UtcNow
                };

                if (roles != null && roles.Any())
                {
                    for (int i = 0; i < roles.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(roles[i]))
                        {
                            tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, roles[i].Trim()));
                        }
                    }
                }

                response.Result = tokenDescriptor;
            }
            catch (Exception ex)
            {
                response.AddError(ex);
            }

            return response;
        }
    }
}
