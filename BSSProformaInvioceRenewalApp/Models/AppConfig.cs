namespace BSSProformaInvioceRenewalApp.Models
{
    public sealed class AppConfig
    {
        public string? Environment { get; set; }
        public string? APIUrl { get; set; }
        public string? OAuthTokenUrl { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? BSSUsername { get; set; }
        public string? BSSPassword { get; set; }
        public string? MSOauthUrl { get; set; }
        public string? SPAPIUrl { get; set; }
        public string? CertificatePath { get; set; }
        public string? CertificatePassword { get; set; }
        public string? TenantId { get; set; }
        public string? MSClientId { get; set; }
        public string? MSClientSecret { get; set; }
        public string? Scope { get; set; }
        public string? SiteId { get; set; }
        public string? ListId { get; set; }
        public string? InvoiceListId { get; set; }
        public string? InputExcelName { get; set; }
    }
}
