using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Graph_Api
{
    class Exchange
    {
		public async Task GetMailIdsAndPrintDetails(List<string> userIds, string accessToken)
		{
			foreach (var userId in userIds)
			{

				var filterDate = DateTime.UtcNow.AddDays(-60).ToString("yyyy-MM-ddTHH:mm:ssZ");

				var graphApiUrl = $"https://graph.microsoft.com/v1.0/users/{userId}/messages?$top=9999&select=id,lastModifiedDateTime,body,subject,internetMessageHeaders&$filter=lastModifiedDateTime ge {filterDate}";

				using (var httpClient = new HttpClient())
				{
					httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
					httpClient.DefaultRequestHeaders.Add("Prefer", "outlook.body-content-type='html'");

					var response = await httpClient.GetAsync(graphApiUrl);
					if (response.IsSuccessStatusCode)
					{
						var content = await response.Content.ReadAsStringAsync();
						dynamic result = JObject.Parse(content);

						foreach (var mail in result.value)
						{
                            Console.WriteLine($"Mail Subject : {mail.subject}");
                            Console.WriteLine($"Mail Subject : {mail.body.content}");
						}
					}
					else
					{
						Console.WriteLine($"Error getting mail info for user {userId}: {response.StatusCode}");
					}
				}

			}
		}


	}
}
