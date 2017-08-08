using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SchoolClient;

namespace ServiceDiscvery.SelfRegisteration.Client
{
    class Program
    {
        private static IConfigurationRoot _configuration;
        private static ApiClient _apiClient;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            LoadConfig();

            _apiClient = new ApiClient(_configuration);

            Cities().Wait();
        }
        private static void LoadConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            _configuration = builder.Build();
        }

        private static async Task Cities()
        {
            var students = await _apiClient.GetStudents();
            Console.WriteLine($"Student Count: {students.Count()}");
        }

    }
}