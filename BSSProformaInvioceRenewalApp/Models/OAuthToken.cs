using System.Text.Json.Serialization;

namespace BSSProformaInvioceRenewalApp.Models
{
    internal class OAuthToken
    {
        [JsonPropertyNameAttribute("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyNameAttribute("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyNameAttribute("token_type")]
        public string TokenType { get; set; }
    }
}
