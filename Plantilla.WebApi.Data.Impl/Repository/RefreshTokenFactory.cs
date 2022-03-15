using Microsoft.Extensions.Configuration;
using Plantilla.WebApi.Data.Contracts.Repository.OAuth;
using System.Collections.Generic;
using System.Linq;

namespace Plantilla.WebApi.Data.Impl.Repository
{

    public enum RefreshEntityType
    {
        Redis,
        Database
    }

    public class RefreshTokenFactory : IRefreshTokenFactory
    {
        private readonly IConfiguration _configuration;
        private readonly IEnumerable<IRefreshTokenRepository> _repositories;

        public RefreshTokenFactory(IConfiguration configuration, IEnumerable<IRefreshTokenRepository> repositories)
        {
            _configuration = configuration;
            _repositories = repositories;
        }

        public IRefreshTokenRepository GetRefreshTokenService()
        {
            return GetRefreshTokenService(_configuration["RefreshTokenSource"]);
        }

        private IRefreshTokenRepository GetRefreshTokenService(string refreshServiceType)
        {
            switch (refreshServiceType)
            {
                case "Redis":
                    return _repositories.FirstOrDefault(c => c.Source == RefreshEntityType.Redis.ToString());
                case "Database":
                    return _repositories.FirstOrDefault(c => c.Source == RefreshEntityType.Database.ToString());
            }

            return null;
        }
    }
}
