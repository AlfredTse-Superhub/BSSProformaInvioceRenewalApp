using System.Text.Json.Serialization;

namespace BSSProformaInvioceRenewalApp.Models
{
    public class Order
    {
        [JsonPropertyNameAttribute("id")]
        public string Id { get; set; } = "";

        [JsonPropertyNameAttribute("code")]
        public string Code { get; set; } = "";
    }
}
