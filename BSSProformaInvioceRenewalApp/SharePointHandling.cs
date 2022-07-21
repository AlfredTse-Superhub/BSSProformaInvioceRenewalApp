using BSSProformaInvioceRenewalApp.Models;
using Microsoft.Graph;

namespace BSSProformaInvioceRenewalApp
{
    public static class SharePointHandling
    {
        private static readonly AppConfig _appConfig = EnvironmentService.GetInstance().GetAppConfig();
        private static readonly int _maxRetries = 3;

        //public static async Task<int> GetInvoiceID()
        //{

        //}

        public static async Task CreateInvoiceLog(GraphServiceClient graphClient, List<Models.Subscription> subscriptionList, string invoiceID)
        {
            try
            {
                foreach (var sub in subscriptionList)
                {
                    ListItem listItem = new()
                    {
                        Fields = new FieldValueSet
                        {
                            AdditionalData = new Dictionary<string, object>()
                                {
                                    { "InvoiceID", invoiceID},
                                    { "SubscriptionID", sub.Id },
                                    { "Customer", sub.Account.Name}
                                }
                        }
                    };
                    await graphClient.Sites[_appConfig.SiteId].Lists[_appConfig.ListId].Items
                        .Request()
                        .WithMaxRetry(_maxRetries)
                        .AddAsync(listItem);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Failed to create log to SP");
            }
        }
    }
}
