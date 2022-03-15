using Plantilla.WebApi.Data.Contracts.Model;
using System;
using System.Threading.Tasks;

namespace Plantilla.WebApi.Data.Contracts.Repository.OAuth
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken, Guid>
    {
        public string Source { get; }
        Task DeleteByJTIAsync(Guid Jti);
        Task<string> GetTokenByJti(Guid Jti);
    }
}
