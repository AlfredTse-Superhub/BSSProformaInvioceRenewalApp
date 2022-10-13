using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BSSProformaInvioceRenewalApp.Models
{
    public class ApiResponse
    {
        [JsonPropertyNameAttribute("data")]
        public List<Subscription> Data { get; set; } = new();

        [JsonPropertyNameAttribute("paging")]
        public Paging Paging { get; set; } = new();

        [JsonPropertyNameAttribute("links")]
        public Link Link { get; set; } = new();
    }

    public class Paging
    {
        [JsonPropertyNameAttribute("page")]
        public int Page { get; set; }

        [JsonPropertyNameAttribute("size")]
        public int Size { get; set; }

        [JsonPropertyNameAttribute("totalPages")]
        public int TotalPages { get; set; }
    }

    public class Link
    {
        [JsonPropertyNameAttribute("self")]
        public string? Self { get; set; }

        [JsonPropertyNameAttribute("first")]
        public string? First { get; set; }

        [JsonPropertyNameAttribute("last")]
        public string? Last { get; set; }

        [JsonPropertyNameAttribute("nect")]
        public string? Next { get; set; }

        [JsonPropertyNameAttribute("previous")]
        public string? Previous { get; set; }

    }
}
