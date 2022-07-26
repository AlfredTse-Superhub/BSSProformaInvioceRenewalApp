using System.Text.Json.Serialization;

namespace BSSProformaInvioceRenewalApp.Models
{
    public class Account
    {
        [JsonPropertyNameAttribute("id")]
        public string Id { get; set; }

        [JsonPropertyNameAttribute("phone")]
        public string Phone { get; set; }

        [JsonPropertyNameAttribute("name")]
        public string Name { get; set; }

        [JsonPropertyNameAttribute("addresses")]
        public List<Address> Addresses { get; set; } = new List<Address>();

        [JsonPropertyNameAttribute("email")]
        public string Email { get; set; }

        [JsonPropertyNameAttribute("paymentMethod")]
        public string PaymentMethod { get; set; }
    }

    public class Address
    {
        [JsonPropertyNameAttribute("address1")]
        public string Address1 { get; set; }

        [JsonPropertyNameAttribute("address2")]
        public string Address2 { get; set; }

        [JsonPropertyNameAttribute("city")]
        public string City { get; set; }

        [JsonPropertyNameAttribute("state")]
        public string State { get; set; }

        [JsonPropertyNameAttribute("country")]
        public string Country { get; set; }
    }
}
