﻿using BSSProformaInvioceRenewalApp.Models;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System.Net.Security;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BSSProformaInvioceRenewalApp
{
    public static class SubscriptionHandling
    {
        private static readonly AppConfig _appConfig = EnvironmentService.GetInstance().GetAppConfig();

        private static RestRequest CreateGetRequest(string token, string versionNo = "3")
        {
            RestRequest request = new RestRequest();
            request.AddHeader("Authorization", token);
            request.AddHeader("X-Api-Version", versionNo);
            request.AddHeader("Content-Type", "application/json");

            return request;
        }

        private static async Task<string> GetOAuthToken()
        {
            string? url = _appConfig.OAuthTokenUrl;
            string? clientId = _appConfig.ClientId;
            string? clientSecret = _appConfig.ClientSecret;
            string? username = _appConfig.BSSUsername;
            string? password = _appConfig.BSSPassword;

            RestClient client = new(url);
            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            client.Authenticator = new HttpBasicAuthenticator(clientId, clientSecret);

            IRestRequest request = new RestRequest(Method.POST)
                .AddHeader("content-type", "application/x-www-form-urlencoded")
                .AddHeader("Authorization", "basic " + client.Authenticator.ToString())
                .AddParameter("application/x-www-form-urlencoded", ($"grant_type=password&username={username}&password={password}"), ParameterType.RequestBody);

            IRestResponse response = await client.ExecuteAsync(request);

            OAuthToken? oAuthToken = JsonSerializer.Deserialize<OAuthToken>(response.Content);
            string token = oAuthToken!.TokenType + " " + oAuthToken.AccessToken;
            if (string.IsNullOrEmpty(token.Trim()))
            {
                throw new Exception("Authentication Failed: Empty token!");
            }
            return token;
        }

        public static bool ValidateSubscriptions(List<Subscription> subscriptionList)
        {
            bool isAllValid = true;
            string firstCustomer = subscriptionList[0].Account.Name;
            int count = 1;

            foreach (var sub in subscriptionList)
            {
                if (sub == null || firstCustomer != sub.Account.Name)
                {
                    isAllValid = false;
                }
                Console.WriteLine($"  {count}) ID: {sub.Id}, valid?: {sub != null}, customer: '{sub.Account.Name}'");
                count++;
            }
            return isAllValid;
        }

        public static async Task<List<Subscription>> FetchSubscriptions(IEnumerable<string> subscriptionIDList)
        {
            try
            {
                string token = await GetOAuthToken();
                // GetSubscriptionTest(token, 140); //test
                List<Subscription> subscriptionList = new();

                foreach (string subscriptionID in subscriptionIDList)
                {
                    Subscription sub = GetSubscription(token, subscriptionID);
                    sub.Addons = FetchSubAddons(token, sub);
                    Account account = GetAccount(token, sub.Account.Id);

                    if (account != null)
                    {
                        sub.Account.Id = account.Id;
                        sub.Customer.PaymentMethod = account.PaymentMethod ?? "";
                        if (account.Addresses.Count() > 0)
                        {
                            Address firstAddress = account.Addresses[0];
                            sub.Customer.Address = (firstAddress.Address1 ?? "") + " " + (firstAddress.Address2 ?? "");
                            sub.Customer.City = firstAddress.City;
                            sub.Customer.Country = firstAddress.Country;
                            sub.Customer.Phone = account.Phone;
                        }
                    }
                    if (sub.Account.Id != sub.BillingTo.Id)
                    {
                        account = GetAccount(token, sub.BillingTo.Id);
                    }
                    if (account != null)
                    {
                        sub.Customer.BillToEmail = account.Email;
                        sub.Customer.BillToContact = account.Name;
                    }

                    PricingInfo pricingInfo = GetPricingInfo(token, sub.Id);
                    sub.Product.FinalUnitPrice = pricingInfo.UnitPrice.Value;

                    subscriptionList.Add(sub);
                }
                return subscriptionList;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: subscription(s) not recognized");
            }
        }

        private static List<Addons> FetchSubAddons(string token, Subscription sub)
        {
            List<Addons> result = GetSubscriptionAddons(token, sub.Id);

            if (result.FirstOrDefault() != null)
            {
                foreach (Addons addon in result)
                {
                    addon.PriceInfo = GetAddonPriceInfo(token, addon.Id, sub.Id);
                }
            }
            return result;
        }

        private static Subscription GetSubscription(string token, string id)
        {
            string url = _appConfig.APIUrl + $"/Subscriptions/{id}";
            RestClient client = new RestClient(url);
            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            RestRequest request = CreateGetRequest(token);
            request.AddQueryParameter("status", "active");
            request.AddQueryParameter("accountId", "0");
            request.AddQueryParameter("size", "25");

            IRestResponse response = client.Execute(request);
            Subscription result = JsonConvert.DeserializeObject<Subscription>(response.Content);
            return result;
        }

        // testing
        //private static Subscription GetSubscriptionTest(string token, int accountId)
        //{
        //    // string url = _appConfig.APIUrl + $"/Accounts/{accountId}/subscriptions/73823ef4-d420-4627-8889-2dd6a3a46dc9";
        //    string url = _appConfig.APIUrl + $"/Accounts/{accountId}/subscriptions";
        //    RestClient client = new RestClient(url);
        //    client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
        //    RestRequest request = CreateGetRequest(token, "2.2");
        //    //request.AddQueryParameter("status", "active");
        //    //request.AddQueryParameter("accountId", "0");
        //    //request.AddQueryParameter("size", "25");

        //    IRestResponse response = client.Execute(request);
        //    Subscription result = JsonConvert.DeserializeObject<Subscription>(response.Content);
        //    return result;
        //}

        private static List<Addons> GetSubscriptionAddons(string token, string id)
        {
            string url = _appConfig.APIUrl + $"/Subscriptions/{id}/addons";
            RestClient client = new(url);
            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            RestRequest request = CreateGetRequest(token);

            IRestResponse response = client.Execute(request);
            List<Addons> result = JsonConvert.DeserializeObject<List<Addons>>(response.Content);
            if (result != null)
            {
                result = result.FindAll(x => x.Status == "Active");
            }
            return result;
        }

        private static PricingInfo GetAddonPriceInfo(string token, string addonId, string subID)
        {
            string url = _appConfig.APIUrl + $"/Subscriptions/{subID}/addons/{addonId}/pricinginfo";
            RestClient client = new(url);
            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            RestRequest request = CreateGetRequest(token);

            IRestResponse response = client.Execute(request);
            PricingInfo result = JsonConvert.DeserializeObject<PricingInfo>(response.Content);
            return result;
        }

        private static Account GetAccount(string token, string id)
        {
            string url = _appConfig.APIUrl + $"/Accounts/{id}";
            RestClient client = new(url);
            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            RestRequest request = CreateGetRequest(token, "2.2");

            IRestResponse response = client.Execute(request);
            Account result = JsonConvert.DeserializeObject<Account>(response.Content);
            return result;
        }

        private static PricingInfo GetPricingInfo(string token, string id)
        {
            string url = _appConfig.APIUrl + $"/Subscriptions/{id}/pricingInfo";
            RestClient client = new(url);
            client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            RestRequest request = CreateGetRequest(token);

            IRestResponse response = client.Execute(request);
            PricingInfo result = JsonConvert.DeserializeObject<PricingInfo>(response.Content);
            return result;
        }
    }
}
