using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Plantilla.WebApi.Busines.Contracts.Service;
using Plantilla.WebApi.Dto;
using System.Threading.Tasks;

namespace Plantilla.WebApi.Controllers.OAuth
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : Controller
    {
        private readonly ITokenService tokenService;
        private readonly ILogger<TokenController> logger;

        public TokenController(ITokenService tokenService,
                                ILogger<TokenController> logger)
        {
            this.tokenService = tokenService;
            this.logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        public async Task<IActionResult> Post([FromBody] TokenRequest tokenRequest)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = await tokenService.GenerateToken(tokenRequest);

            if (result.HasErrors())
            {
                logger.LogError("Invalid credentials for user " + tokenRequest.Username);

                return await Task.FromResult(Unauthorized("Invalid credentials"));
            }
            return Ok(result.Result);
        }

        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            var accessToken = HttpContext.Request.Headers["Authorization"][0];
            var result = await tokenService.RefreshToken(accessToken.Substring(accessToken.IndexOf(" ") + 1), refreshToken);

            return Ok(result);
        }
    }
}
