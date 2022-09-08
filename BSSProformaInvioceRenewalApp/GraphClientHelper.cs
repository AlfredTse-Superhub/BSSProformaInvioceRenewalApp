using Azure.Identity;
using BSSProformaInvioceRenewalApp.Models;
using Microsoft.Graph;

namespace BSSProformaInvioceRenewalApp
{
    public class GraphClientHelper
    {
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

