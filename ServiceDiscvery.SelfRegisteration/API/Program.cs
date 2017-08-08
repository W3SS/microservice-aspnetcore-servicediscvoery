using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ServiceDiscvery.SelfRegisteration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var freePort = FreeTcpPort();

            var host = new WebHostBuilder()
                .UseUrls($"http://localhost:{freePort}")
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            var loggingFactory = host.Services.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
            var logger = loggingFactory.CreateLogger(nameof(Program));
            logger.LogInformation($"{Process.GetCurrentProcess().Id}");

            host.Run();
        }

        private static int FreeTcpPort()
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            var port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
    }
}
