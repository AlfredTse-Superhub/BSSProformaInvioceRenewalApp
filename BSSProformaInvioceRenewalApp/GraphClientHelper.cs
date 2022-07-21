using Azure.Identity;
using BSSProformaInvioceRenewalApp.Models;
using Microsoft.Graph;

namespace BSSProformaInvioceRenewalApp
{
    public class GraphClientHelper
    {
        //private static string tenantID = Environment.GetEnvironmentVariable("APPSETTING_tenantID");
        //private static string clientID = Environment.GetEnvironmentVariable("APPSETTING_MS_client_id");
        //private static string clientSecret = Environment.GetEnvironmentVariable("APPSETTING_MS_client_secret");
        //private static string scope = Environment.GetEnvironmentVariable("APPSETTING_scope");
        private static readonly AppConfig _appConfig = EnvironmentService.GetInstance().GetAppConfig();

        public static GraphServiceClient GetGraphClient()
        {
            string[] scopes = new[] { _appConfig.Scope };
            TokenCredentialOptions options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };
            ClientSecretCredential clientSecretCredential = new ClientSecretCredential(
                _appConfig.TenantId,
                _appConfig.MSClientId,
                _appConfig.MSClientSecret,
                options
            );
            return new GraphServiceClient(clientSecretCredential, scopes);
        }
    }
}

