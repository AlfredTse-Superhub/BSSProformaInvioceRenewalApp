using System.Text.Json.Serialization;

namespace BSSProformaInvioceRenewalApp.Models
{
    public class PricingInfo
    {
        [JsonPropertyNameAttribute("unit")]
        public Detail Unit { get; set; }

        [JsonPropertyNameAttribute("unitPrice")]
        public UnitPrice UnitPrice { get; set; }
    }

    public class UnitPrice
    {
        [JsonPropertyNameAttribute("value")]
        public string Value { get; set; }

        [JsonPropertyNameAttribute("isUserDefined")]
        public string IsUserDefined { get; set; }
    }
}
