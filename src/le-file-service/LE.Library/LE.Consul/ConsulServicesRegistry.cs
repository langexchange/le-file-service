using Consul;
using LE.Library.LE.Consul.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace LE.Library.LE.Consul
{
    public class ConsulServicesRegistry : IConsulServicesRegistry
    {
        private readonly IConsulClient _client;
        private readonly IDictionary<string, ISet<string>> _usedServices = new Dictionary<string, ISet<string>>();
        private Dictionary<string, AgentService> _listServices;
        private AgentServiceRegistration _registration;
        private ILogger<ConsulServicesRegistry> _logger;

        public ConsulServicesRegistry(IConsulClient client, ILogger<ConsulServicesRegistry> logger)
        {
            _client = client;
            _listServices = new Dictionary<string, AgentService>();
            _logger = logger;
        }

        public Task<AgentService> GetAsync(string name)
        {
            var services = GetServices(_listServices, name);
            if (!services.Any())
            {
                return Task.FromResult<AgentService>(null);
            }
            if (!_usedServices.ContainsKey(name))
            {
                _usedServices[name] = new HashSet<string>();
            }
            else if (services.Count == _usedServices[name].Count)
            {
                _usedServices[name].Clear();
            }

            return Task.FromResult(GetService(services, name));
        }

        private AgentService GetService(IList<AgentService> services, string name)
        {
            AgentService service = null;
            var unusedServices = services.Where(s => !_usedServices[name].Contains(s.ID)).ToList();
            if (unusedServices.Any())
            {
                service = unusedServices[RandomNumberGenerator.GetInt32(unusedServices.Count)];
            }
            else
            {
                service = services.First();
                _usedServices[name].Clear();
            }
            _usedServices[name].Add(service.ID);

            return service;
        }
        private IList<AgentService> GetServices(IDictionary<string, AgentService> allServices, string name)
            => allServices?.Where(s => s.Value.Service.Equals(name,
                       StringComparison.InvariantCultureIgnoreCase))
                   .Select(x => x.Value).ToList() ?? new List<AgentService>();

        public void SetupConsul(ConsulOptions consulOptions, IApplicationBuilder app)
        {
            var address = consulOptions.Address;
            if (string.IsNullOrWhiteSpace(address))
            {
                var features = app.Properties["server.Features"] as FeatureCollection;
                var addresses = features.Get<IServerAddressesFeature>();
                address = addresses.Addresses.First();
            }
            var uri = new Uri(address);
            _registration = new AgentServiceRegistration
            {
                Name = consulOptions.Service,
                ID = $"{consulOptions.Service}:{Guid.NewGuid().ToString("n")}",
                Address = $"{uri.Host}",
                Port = uri.Port,
            };
            if (consulOptions.PingEnabled)
            {
                var pingEndpoint = consulOptions.PingEndpoint;
                var pingInterval = consulOptions.PingInterval <= 0 ? 5 : consulOptions.PingInterval;
                var removeAfterInterval =
                    consulOptions.RemoveAfterInterval <= 0 ? 10 : consulOptions.RemoveAfterInterval;
                var httpCheck = new AgentServiceCheck
                {
                    Interval = TimeSpan.FromSeconds(pingInterval),
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(removeAfterInterval),
                    HTTP = $"{uri.Scheme}://{uri.Host}{(_registration.Port > 0 ? $":{_registration.Port}" : string.Empty)}/{pingEndpoint}"
                };
                //_registration.Checks = new[] { httpCheck };

                if (string.IsNullOrWhiteSpace(pingEndpoint) || pingEndpoint.ToLower() == "ping")
                {
                    pingEndpoint = "ping";
                    app.Use(async (ctx, next) =>
                    {
                        if (ctx.Request.Path.Equals(new PathString($"/{pingEndpoint}")))
                        {
                            ctx.Response.StatusCode = StatusCodes.Status200OK;
                            await ctx.Response.WriteAsync("ok");
                            return;
                        }
                        await next.Invoke();
                    });
                }
            }
        }
        //public void SetupConsul(ConsulOptions consulOptions, IApplicationBuilder app)
        //{
        //    var address = consulOptions.Address;
        //    if (string.IsNullOrWhiteSpace(address))
        //    {
        //        var ip = ConsulExtensions.GetPrivateAddress();
        //        if (ip == null)
        //            throw new ArgumentException($"{ip} Consul Client address can not be empty.",
        //            nameof(consulOptions.PingEndpoint));
        //        address = ip.ToString();
        //    }
        //    _registration = new AgentServiceRegistration
        //    {
        //        Name = consulOptions.Service,
        //        ID = $"{consulOptions.Service}:{Guid.NewGuid().ToString("n")}",
        //        Address = address,
        //        Port = consulOptions.Port
        //    };
        //    if (consulOptions.PingEnabled)
        //    {
        //        var pingEndpoint = consulOptions.PingEndpoint;
        //        var pingInterval = consulOptions.PingInterval <= 0 ? 5 : consulOptions.PingInterval;
        //        var removeAfterInterval =
        //            consulOptions.RemoveAfterInterval <= 0 ? 10 : consulOptions.RemoveAfterInterval;
        //        var scheme = address.StartsWith("http", StringComparison.InvariantCultureIgnoreCase) ? string.Empty : "http://";
        //        var httpCheck = new AgentServiceCheck
        //        {
        //            Interval = TimeSpan.FromSeconds(pingInterval),
        //            DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(removeAfterInterval),
        //            HTTP = $"{scheme}{address}{(_registration.Port > 0 ? $":{_registration.Port}" : string.Empty)}/{pingEndpoint}"
        //        };
        //        //_registration.Checks = new[] { httpCheck };

        //        if (string.IsNullOrWhiteSpace(pingEndpoint) || pingEndpoint.ToLower() == "ping")
        //        {
        //            pingEndpoint = "ping";
        //            app.Use(async (ctx, next) =>
        //            {
        //                if (ctx.Request.Path.Equals(new PathString($"/{pingEndpoint}")))
        //                {
        //                    ctx.Response.StatusCode = StatusCodes.Status200OK;
        //                    await ctx.Response.WriteAsync("ok");
        //                    return;
        //                }
        //                await next.Invoke();
        //            });
        //        }
        //    }
        //}

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogError("Start register service, message");
                    var list = await _client.Agent.Services(cancellationToken);
                    _listServices = list.Response;
                    if (!list.Response.ContainsKey(_registration.Name) || !list.Response.Values.Any(x => x.ID == _registration.ID))
                    {
                        _logger.LogInformation("register service name: {0}, address: {1}, port: {2}", _registration.Name, _registration.Address, _registration.Port);
                        await _client.Agent.ServiceRegister(_registration, cancellationToken);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError("Faile register service, message: {0}", e.Message);
                    _listServices = new Dictionary<string, AgentService>();
                }
                await Task.Delay(TimeSpan.FromSeconds(60));
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stop service name: {0}, id: {1}", _registration.Name, _registration.ID);
            return _client.Agent.ServiceDeregister(_registration.ID);
        }
    }
}
