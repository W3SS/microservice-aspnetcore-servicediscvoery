using System;
using System.Linq;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;

namespace ServiceDiscvery.SelfRegisteration.Services
{
    public static class Extensions
    {
        public static IApplicationBuilder RegisterYouOwnAsAService(this IApplicationBuilder app, IApplicationLifetime lifetime)
        {
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
            var consulConfig = app.ApplicationServices.GetRequiredService<IOptions<ConsulConfig>>();

            var loggingFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var logger = loggingFactory.CreateLogger<IApplicationBuilder>();

            try
            {
                var features = app.Properties["server.Features"] as FeatureCollection;
                var addresses = features.Get<IServerAddressesFeature>();
                var address = addresses.Addresses.First();

                var uri = new Uri(address);
                var registration = new AgentServiceRegistration()
                {
                    ID = $"{consulConfig.Value.Id}-{uri.Port}",
                    Name = consulConfig.Value.Name,
                    Address = $"{uri.Scheme}://{uri.Host}",
                    Port = uri.Port,
                    Tags = new[] { "City", "Hotel", "API" },
                    EnableTagOverride = true,
                    Check = new AgentServiceCheck()
                    {                        
                        HTTP = $"{uri.Scheme}://{uri.Host}:{uri.Port}/api/heartbeat/status",
                        Timeout = TimeSpan.FromSeconds(3),
                        Interval = TimeSpan.FromSeconds(10),
                        TTL = TimeSpan.FromMinutes(1),
                        DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(5)
                    }
                };

                logger.LogInformation("Start Self Client Registering from Consul");

                //First deregister service from Consul
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();

                //Then deregister service from Consul
                consulClient.Agent.ServiceRegister(registration).Wait();
                logger.LogInformation("End Self Client Registering from Consul");

                lifetime.ApplicationStopping.Register(() =>
                {
                    logger.LogInformation("Deregistering from Consul");
                    consulClient.Agent.ServiceDeregister(registration.ID).Wait();
                });
            }
            catch (Exception x)
            {
                logger.LogCritical(x.ToString());
            }

            return app;
        }
    }
}
