namespace Plantilla.WebApi.Data.Contracts.Repository.OAuth
{
    public interface IRefreshTokenFactory
    {
        public IRefreshTokenRepository GetRefreshTokenService();

    }
}
