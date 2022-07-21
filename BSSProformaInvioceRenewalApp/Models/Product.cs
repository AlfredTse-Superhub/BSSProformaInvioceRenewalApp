using System.Text.Json.Serialization;

namespace BSSProformaInvioceRenewalApp.Models
{
    public class Product
    {
        [JsonPropertyNameAttribute("id")]
        public string Id { get; set; }

        [JsonPropertyNameAttribute("name")]
        public string Name { get; set; }

        [JsonPropertyNameAttribute("code")]
        public string Code { get; set; }

        public string FinalUnitPrice { get; set; }
    }
}
