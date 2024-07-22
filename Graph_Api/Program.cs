using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph_Api
{
    class Program
    {
        static async void Main(string[] args)
        {
            await ClassifyMails();
        }
        public static async Task ClassifyMails()
        {

            Console.WriteLine("Started");

            Setter.InitializeGraphForAppOnlyAuth(Setter.Settings.LoadSettings("your_client_id", "your_tenant_id", "your_client_secret"));

            var accessToken = await Setter.GetAppOnlyTokenAsync();

            List<string> userIds = await Setter.GetUserIds(accessToken);
            Exchange exchange = new Exchange();
            await exchange.GetMailIdsAndPrintDetails(userIds, accessToken);
            Console.ReadLine();

        }
    }
}
