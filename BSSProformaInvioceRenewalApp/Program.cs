﻿// See https://aka.ms/new-console-template for more information

using BSSProformaInvioceRenewalApp.Models;
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

                    // Valiadate subscriptions
                    List<Subscription> subscriptionList = await SubscriptionHandling.FetchSubscriptions(subscriptionIDList.Distinct());
                    bool isValid = SubscriptionHandling.ValidateSubscriptions(subscriptionList);
                    if (!isValid)
                    {
                        throw new Exception("Error: Invalid inputs!");
                    }

                    // Check invoiceID duplication, generate PDF and log to SP
                    Console.WriteLine("\nGenerating PDF...");
                    GraphServiceClient graphClient = GraphClientHelper.GetGraphClient();
                    int maxRetry = 5;
                    int count = 0;
                    do
                    {
                        try
                        {
                            count++;
                            string invoiceDigits = await SharePointHandling.GetNextInvoiceDigits(graphClient);
                            string invoiceID = string.Concat("INV/M/", DateTime.Now.Year.ToString().AsSpan(2), invoiceDigits);
                            await SharePointHandling.CreateInvoiceLog(graphClient, invoiceID, invoiceDigits);
                            await SharePointHandling.CreateSubscriptionLog(graphClient, subscriptionList, invoiceID);
                            InvoiceHandling.GeneratePDF(subscriptionList, invoiceID);
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            if (count == maxRetry)
                            {
                                throw new Exception("Error: Action failed, please try again or contact developer for help.");
                            }
                            continue;
                        }
                    }
                    while(count < maxRetry);

                    Console.WriteLine($"Completed! The file is saved in the PDF folder \n\n\n");

                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n"+ ex.Message + "\n\n\n");
                    continue;
                }
            } 
            while (true);
        }
    }
}
