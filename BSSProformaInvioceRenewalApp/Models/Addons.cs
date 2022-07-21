using System.Text.Json.Serialization;

namespace BSSProformaInvioceRenewalApp.Models
{
    public class Addons
    {
        [JsonPropertyNameAttribute("id")]
        public string Id { get; set; }

        [JsonPropertyNameAttribute("product")]
        public Product Product { get; set; }

        [JsonPropertyNameAttribute("quantity")]
        public string Quantity { get; set; }

        [JsonPropertyNameAttribute("status")]
        public string Status { get; set; }

        public PricingInfo PriceInfo { get; set; }
    }
}
