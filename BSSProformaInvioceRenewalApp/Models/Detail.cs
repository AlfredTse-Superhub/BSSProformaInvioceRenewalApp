using System.Text.Json.Serialization;

namespace BSSProformaInvioceRenewalApp.Models
{
    public class Detail
    {
        [JsonPropertyNameAttribute("id")]
        public string Id { get; set; }

        [JsonPropertyNameAttribute("name")]
        public string Name { get; set; }
    }
}
