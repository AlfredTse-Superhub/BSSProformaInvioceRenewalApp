using System.Text.Json.Serialization;

namespace BSSProformaInvioceRenewalApp.Models
{
    public class BillingToContact
    {
        [JsonPropertyNameAttribute("id")]
        public string Id { get; set; }

        [JsonPropertyNameAttribute("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyNameAttribute("lastName")]
        public string LastName { get; set; }
    }
}
