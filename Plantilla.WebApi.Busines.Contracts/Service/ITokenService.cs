using Plantilla.WebApi.Busines.Contracts.Domain;
using Plantilla.WebApi.Dto;
using System.Threading.Tasks;

namespace Plantilla.WebApi.Busines.Contracts.Service
{
    public interface ITokenService
    {
        Task<OperationResult<TokenResponse>> GenerateToken(TokenRequest request);

        Task<OperationResult<TokenResponse>> RefreshToken(string accessToken, string refreshToken);
    }
}