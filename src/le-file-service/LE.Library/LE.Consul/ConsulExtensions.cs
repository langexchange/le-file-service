using Consul;
using LE.Library.LE.Consul.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace LE.Library.LE.Consul
{
    public static class ConsulExtensions
    {
        private static readonly string ConsulSectionName = "Consul";
        public static IPAddress GetPrivateAddress()
        {
            var name = Dns.GetHostName(); // get container id
            return Dns.GetHostEntry(name).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
        }

        public static ConsulOptions AddConsul(this IServiceCollection services)
        {
            IConfiguration configuration;
            using (var serviceProvider = services.BuildServiceProvider())
            {
                configuration = serviceProvider.GetService<IConfiguration>();
            }

            var options = new ConsulOptions();
            configuration.GetSection(ConsulSectionName).Bind(options);
            options.Url = Environment.GetEnvironmentVariable("CONSUL_URL");
            //services.Configure<ConsulOptions>(configuration.GetSection(ConsulSectionName));
            services.Configure<ConsulOptions>(op =>
            {
                configuration.GetSection(ConsulSectionName).Bind(op);
                op.Url = Environment.GetEnvironmentVariable("CONSUL_URL");
            });
            services.AddSingleton<IConsulServicesRegistry, ConsulServicesRegistry>();
            services.AddTransient<ConsulServiceDiscoveryMessageHandler>();
            services.AddHttpClient<IConsulHttpClient, ConsulHttpClient>()
                .AddHttpMessageHandler<ConsulServiceDiscoveryMessageHandler>();
                //.AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.CircuitBreakerAsync(
                //    handledEventsAllowedBeforeBreaking: 5,
                //    durationOfBreak: TimeSpan.FromMinutes(1)
                //));

            services.AddSingleton<IConsulClient>(c => new ConsulClient(cfg =>
            {
                if (!string.IsNullOrEmpty(options.Url))
                {
                    cfg.Address = new Uri(options.Url);
                }
            }));
            return options;
        }

        //Returns unique service ID used for removing the service from registry.
        public static void UseConsul(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            try
            {
                var consulOptions = scope.ServiceProvider.GetService<IOptions<ConsulOptions>>();
                //if (consulOptions?.Value != null)
                //{
                    var consulServicesRegistry = scope.ServiceProvider.GetService<IConsulServicesRegistry>();
                    var lifeTime = scope.ServiceProvider.GetService<IHostApplicationLifetime>();
                    consulServicesRegistry.SetupConsul(consulOptions.Value, app);
                    var tokenResource = new CancellationTokenSource();
                    consulServicesRegistry.StartAsync(tokenResource.Token);
                    lifeTime.ApplicationStopped.Register(() =>
                    {
                        tokenResource.Cancel();
                        consulServicesRegistry.StopAsync(tokenResource.Token);
                    });
                //}
            }
            catch (Exception)
            {
            }
        }
    }
}
