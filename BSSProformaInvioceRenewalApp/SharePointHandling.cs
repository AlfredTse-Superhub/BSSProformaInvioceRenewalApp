using BSSProformaInvioceRenewalApp.Models;
using Microsoft.Graph;
// using Microsoft.SharePoint.Client;
// using Microsoft.SharePoint.Client;

namespace BSSProformaInvioceRenewalApp
{
    public static class SharePointHandling
    {
        private static readonly AppConfig _appConfig = EnvironmentService.GetInstance().GetAppConfig();
        private static readonly int _maxRetries = 3;

        public static async Task<string> GetNextInvoiceDigits(GraphServiceClient graphClient)
        {
            try
            {
                var invoiceDigits = "00000001";
                var queryOptions = new List<QueryOption>()
                {
                    new QueryOption("expand", "fields(select=InvoiceDigits)"),
                    new QueryOption("top", "5000")
                };
                var listItems = await graphClient.Sites[_appConfig.SiteId].Lists[_appConfig.InvoiceListId].Items
                     .Request(queryOptions)
                     .WithMaxRetry(_maxRetries)
                     .GetAsync();

                if (listItems.Count() > 0)
                {
                    int digits = listItems.Max(x => int.Parse(x.Fields.AdditionalData.LastOrDefault().Value.ToString()));
                    invoiceDigits = (digits + 1).ToString("D8");
                }

                return invoiceDigits;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Failed to get Invoice ID");
            }
        }

        public static async Task CreateInvoiceLog(GraphServiceClient graphClient, string invoiceID, string invoiceDigits)
        {
            try
            {
                ListItem listItem = new()
                {
                    Fields = new FieldValueSet
                    {
                        AdditionalData = new Dictionary<string, object>()
                            {
                                { "InvoiceID", invoiceID },
                                { "Year", DateTime.Now.Year.ToString() },
                                { "InvoiceDigits", invoiceDigits }
                            }
                    }
                };
                await graphClient.Sites[_appConfig.SiteId].Lists[_appConfig.InvoiceListId].Items
                    .Request()
                    .WithMaxRetry(_maxRetries)
                    .AddAsync(listItem);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Failed to log invoice record to SP");
            }
        }

        public static async Task CreateSubscriptionLog(GraphServiceClient graphClient, List<Models.Subscription> subscriptionList, string invoiceID)
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
                                    { "SubscriptionName", sub.Name },
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
                throw new Exception("Error: Failed to log subscription record to SP");
            }
        }
    }
}
