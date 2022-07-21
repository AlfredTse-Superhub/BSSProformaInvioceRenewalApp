// See https://aka.ms/new-console-template for more information

using BSSProformaInvioceRenewalApp.Models;
using CSharpVitamins;
using GraphServiceClient = Microsoft.Graph.GraphServiceClient;

namespace BSSProformaInvioceRenewalApp
{
    class Program
    {
        static async Task Main()
        {
            Console.WriteLine("\n =========================================================================================");
            Console.WriteLine(" |  You are using BSS Proforma Invoice console application V1.0!                          |");
            Console.WriteLine(" |----------------------------------------------------------------------------------------|");
            Console.WriteLine(" |  Step1:  |  Enter 1 or more subscription ID from the same customer.                    |");
            Console.WriteLine(" |  Step2:  |  A pdf copy of invoice will be saved in the directory of this app.          |");
            Console.WriteLine(" =========================================================================================\n");

            do
            {
                try
                {
                    Console.WriteLine("Please input subscription ID, use spacing for multiple IDs:id1 id2 id3.\n");
                    string? subscriptionIDInput = Console.ReadLine();
                    Console.WriteLine();

                    if (string.IsNullOrWhiteSpace(subscriptionIDInput))
                    {
                        throw new Exception("Warning: Empty inputs!");
                    }
                    IEnumerable<string> subscriptionIDList = subscriptionIDInput
                        .Split()
                        .Where(s => s != string.Empty);

                    List<Subscription> subscriptionList = await SubscriptionHandling.FetchSubscriptions(subscriptionIDList.Distinct());
                    bool isValid = SubscriptionHandling.ValidateSubscriptions(subscriptionList);

                    if (!isValid)
                    {
                        throw new Exception("Error: Invalid inputs!");
                    }

                    Console.WriteLine("\nGenerating PDF...");
                    string invoiceID = ShortGuid.NewGuid().ToString();
                    InvoiceHandling.GeneratePDF(subscriptionList, invoiceID);

                    Console.WriteLine("Logging to SharePoint...");
                    GraphServiceClient graphClient = GraphClientHelper.GetGraphClient();
                    await SharePointHandling.CreateInvoiceLog(graphClient, subscriptionList, invoiceID);

                    Console.WriteLine($"Completed! The file is saved in the PDF folder \n\n\n");

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + "\n\n\n");
                    continue;
                }
            } while (true);
        }
    }
}
