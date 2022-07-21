using System.Text.Json.Serialization;

namespace BSSProformaInvioceRenewalApp.Models
{
    public class Subscription
    {
        [JsonPropertyNameAttribute("id")]
        public string Id { get; set; }

        [JsonPropertyNameAttribute("name")]
        public string Name { get; set; }

        [JsonPropertyNameAttribute("status")]
        public string Status { get; set; }

        [JsonPropertyNameAttribute("order")]
        public Order Order { get; set; } = new Order();

        [JsonPropertyNameAttribute("account")]
        public Detail Account { get; set; }

        [JsonPropertyNameAttribute("billingTo")]
        public Detail BillingTo { get; set; }

        [JsonPropertyNameAttribute("billToContact")]
        public BillingToContact BillToContact { get; set; }

        [JsonPropertyNameAttribute("startDate")]
        public string StartDate { get; set; }

        [JsonPropertyNameAttribute("endDate")]
        public string EndDate { get; set; }

        [JsonPropertyNameAttribute("product")]
        public Product Product { get; set; }

        [JsonPropertyNameAttribute("unit")]
        public Detail Unit { get; set; }

        [JsonPropertyNameAttribute("quantity")]
        public string Quantity { get; set; }

        [JsonPropertyNameAttribute("addons")]
        public List<Addons> Addons { get; set; } = new List<Addons>();

        public string RefID { get; set; }

        public Customer Customer { get; set; } = new Customer();
    }
}
