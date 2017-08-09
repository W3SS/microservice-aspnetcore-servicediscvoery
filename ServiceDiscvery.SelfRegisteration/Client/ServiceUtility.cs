using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ServiceDiscvery.SelfRegisteration.Client
{
    public static class ServiceUtility
    {
        private static IConsulClient consulClient;
        private static IConfigurationRoot configuration;
        internal static List<AgentService> Services;
        private static HttpClient _apiClient;
        internal static void Initialize(IConfigurationRoot config)
        {
            configuration = config;

            consulClient = new ConsulClient(c =>
            {
                var uri = new Uri(configuration["consulConfig:address"]);
                c.Address = uri;
            });
            Services = new List<AgentService>();
        }

        internal static async Task GetAllServiceUrls()
        {
            var services =consulClient.Agent.Services().Result.Response;

            foreach (var service in services)
            {
                if (service.Value.Tags.Any() && service.Value.Tags.Contains("Hotel"))
                    Services.Add(service.Value);
            }
        }

        internal static async Task ShowAllServices()
        {

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("ID                                   Service               Tags                 Port               Address                 EnableTagOverride");
            Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------------------------------------------");
            foreach (var service in Services)
            {
                Console.Write(service.ID+"            ");
                Console.Write(service.Service + "            ");
                Console.Write(service.Tags + "            ");
                Console.Write(service.Port + "            ");
                Console.Write(service.Address + "            ");
                Console.Write(service.EnableTagOverride + "            ");
                Console.WriteLine();
            }
        }

        internal async static Task CallApi()
        {
            ShowAllServices();
            Console.Write("Please enter service name : ");
            var serviceName = Console.ReadLine();

            _apiClient = new HttpClient();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var serviceAgent = Services.FirstOrDefault(s => s.Service.ToLower() == serviceName);
            if(serviceAgent == null)
            {
                Console.WriteLine($"There is no any service with name ={serviceName} ");
                return;
            }
            var response = await _apiClient.GetAsync(new Uri($"{serviceAgent.Address}:{serviceAgent.Port}/api/city")).ConfigureAwait(false);

            Console.WriteLine(response);

        }

        internal static async Task CheckHealthAsync()
        {
            var checks = await consulClient.Health.Service(configuration["consulConfig:serviceName"]);

            foreach (var entry in checks.Response.Where(a=>a.Service.Service.ToLower() == "hotel-api"))
            {
                var check = entry.Checks.FirstOrDefault();
                if (check == null)
                    continue;

                if (check.Status != HealthStatus.Passing)
                    Console.ForegroundColor = ConsoleColor.Red;

                Console.Write($"Node Address is {entry.Node.Address}  ");
                Console.Write($"Service name is {entry.Service.Service}  ");
                Console.Write($"Service Status {check.Status}   ");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;

                if (check.Status == HealthStatus.Passing && !Services.Contains(entry.Service))
                    Services.Add(entry.Service);

                else if (Services.Contains(entry.Service))
                    Services.Remove(entry.Service);
            }

        }
    }
}
