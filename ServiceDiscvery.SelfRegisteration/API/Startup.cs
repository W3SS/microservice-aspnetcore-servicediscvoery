using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceDiscvery.SelfRegisteration.Services;
using Consul;
using Swashbuckle.AspNetCore.Swagger;

namespace ServiceDiscvery.SelfRegisteration
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Repository>();

            //Register ConsulConfig By Consul Configuration info from appsettings.json
            services.Configure<ConsulConfig>(Configuration.GetSection("consulConfig"));

            //Register Consul Client
            services.AddSingleton<IConsulClient,ConsulClient>(CreateConsulClient);

            // Add framework services.
            services.AddMvc();

            //Add Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Servicec.Discovery.SelfRegistration(HotelApi)",
                    Description = "A demo API "
                });
            });
        }

        private ConsulClient CreateConsulClient(IServiceProvider arg)
        {
            return new ConsulClient(consulConfig =>
            {
                consulConfig.Address = new Uri(Configuration["consulConfig:Url"]);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime lifetime)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();

            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
            });

            app.UseWelcomePage();

            app.RegisterYouOwnAsAService(lifetime);
        }
    }
}
