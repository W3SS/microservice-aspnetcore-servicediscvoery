using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ServiceDiscvery.SelfRegisteration.Client
{
    class Program
    {
        static void Main() => Run().Wait();
        #region 
        private static IConfigurationRoot _configuration;
        static async Task Run()
        {
            Console.Title = "ServiceDiscovery pattern. By Masoud Bahrami";

            PrintBanner();

            BuildConfig();

            ServiceUtility.Initialize(_configuration);
            await ServiceUtility.GetAllServiceUrls();
            await ShowMenuAsync();
        }
        private static void PrintBanner()
        {
            Console.WriteLine("Sample Microservice service discovery using asp net core !");
        }
        private static async Task ShowMenuAsync()
        {
            Console.WriteLine("Please Choice !");
            Console.WriteLine("1- Show All Services");
            Console.WriteLine("2- Check Health");
            Console.WriteLine("3- Select one service instance");
            Console.Write("Your choice : ");
            var input = Console.ReadLine();
            var choise = int.Parse(input.ToString());
            await ParseUserCommandAsync(choise);
        }

        private static async Task ParseUserCommandAsync(int choise)
        {
            switch (choise)
            {
                case (1):
                    await ServiceUtility.ShowAllServices();
                    break;
                case (2):
                    await ServiceUtility.CheckHealthAsync();
                    break;
                case (3):
                    await ServiceUtility.CallApi();
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalide Choise ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.ReadKey();
                    Continue();
                    break;
            }
            Console.WriteLine("Press any key to continue ");
            Console.ReadKey();
            Continue();
        }

        private static void Continue()
        {
            Console.Clear();
            PrintBanner();
            ShowMenuAsync();
        }

        private static void BuildConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            _configuration = builder.Build();
        }
        #endregion
    }
}