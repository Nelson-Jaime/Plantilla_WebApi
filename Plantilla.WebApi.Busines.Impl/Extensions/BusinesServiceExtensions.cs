using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Plantilla.WebApi.Busines.Contracts.Domain;
using Plantilla.WebApi.Busines.Contracts.Security;
using Plantilla.WebApi.Busines.Contracts.Service;
using Plantilla.WebApi.Busines.Impl.Service;
using Plantilla.WebApi.Data.Contracts.Repository;
using Plantilla.WebApi.Data.Contracts.Repository.OAuth;
using Plantilla.WebApi.Data.Impl.Extensions;
using System.Collections.Generic;

namespace Plantilla.WebApi.Busines.Impl.Extensions
{
    public static class BusinesServiceExtensions
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDataExtension(configuration);
            services.Configure<Dictionary<ErrorType, ErrorObject>>(dic => configuration.GetSection("BusinessErrors"));
            services.AddTransient<IUserService, UserService>();
            //services.AddTransient<IRedisCache, RedisCache>();

            var jwtTokenConfig = configuration.GetSection("jwt").Get<JwtTokenConfig>();

            services.AddTransient<ITokenService>(ts =>
            {
                return new TokenService(jwtTokenConfig,
                                         ts.GetRequiredService<IUserRepository>(),
                                         ts.GetRequiredService<IUserRoleRepository>(),
                                         ts.GetRequiredService<IRoleRepository>(),
                                         ts.GetRequiredService<IRefreshTokenRepository>(),
                                         configuration,
                                         ts.GetRequiredService<IRefreshTokenFactory>());
            });

            return services;
        }
    }
}
