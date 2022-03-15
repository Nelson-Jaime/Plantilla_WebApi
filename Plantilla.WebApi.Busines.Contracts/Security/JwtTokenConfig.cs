namespace Plantilla.WebApi.Busines.Contracts.Security
{
    public class JwtTokenConfig
    {
        public string Type { get; set; }
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int AccessTokenExpiration { get; set; }
        public int RefreshTokenExpiration { get; set; }


    }
}
