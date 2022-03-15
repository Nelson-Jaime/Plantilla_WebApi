using System.Text.Json.Serialization;

namespace Plantilla.WebApi.Dto
{
    public class TokenRequest
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
