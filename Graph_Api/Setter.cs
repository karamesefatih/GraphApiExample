using Azure.Core;
using Azure.Identity;
using Microsoft.Graph;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Graph_Api
{
    class Setter
    {
        private static List<string> folderIds = new List<string>();
        public static int numberOfApiCalls;
        private static Settings _settings;
        private static ClientSecretCredential _clientSecretCredential;
        public static GraphServiceClient _appClient;
        public class Settings
        {
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
            public string TenantId { get; set; }

            public static Settings LoadSettings(string clientId, string tenantId, string clientSecret)
            {
                var settings = new Settings();
                settings.ClientId = clientId;
                settings.ClientSecret = clientSecret;
                settings.TenantId = tenantId;
                return settings;
            }
        }
        public static void InitializeGraphForAppOnlyAuth(Settings settings)
        {
            _settings = settings;

            // Ensure settings isn't null
            _ = settings ??
                throw new System.NullReferenceException("Settings cannot be null");

            _settings = settings;

            if (_clientSecretCredential == null)
            {
                _clientSecretCredential = new ClientSecretCredential(
                    _settings.TenantId, _settings.ClientId, _settings.ClientSecret);
            }

            if (_appClient == null)
            {
                _appClient = new GraphServiceClient(_clientSecretCredential,
                    // Use the default scope, which will request the scopes
                    // configured on the app registration
                    new[] { "https://graph.microsoft.com/.default" });
            }
        }
        public static async Task<string> GetAppOnlyTokenAsync()
        {
            _ = _clientSecretCredential ??
                throw new System.NullReferenceException("Graph has not been initialized for app-only auth");

            var context = new TokenRequestContext(new[] { "https://graph.microsoft.com/.default" });
            var response = await _clientSecretCredential.GetTokenAsync(context);
            return response.Token;
        }
        public static async Task<List<string>> GetUserIds(string accessToken)
        {
            var userIds = new List<string>();
            var graphApiUrl = "https://graph.microsoft.com/v1.0/users?$select=id&$top=999";

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                string nextLink = graphApiUrl;

                while (!string.IsNullOrEmpty(nextLink))
                {
                    var response = await httpClient.GetAsync(nextLink);
                    if (response.IsSuccessStatusCode)
                    {
                        numberOfApiCalls++;

                        var content = await response.Content.ReadAsStringAsync();
                        dynamic result = JObject.Parse(content);

                        foreach (var user in result.value)
                        {
                            userIds.Add((string)user.id);
                        }

                        nextLink = result["@odata.nextLink"];
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode}");
                        break;
                    }
                }
            }

            return userIds;
        }
    }
}
