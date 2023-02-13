using Consul;
using Microsoft.AspNetCore.Builder;
using System.Threading;
using System.Threading.Tasks;

namespace LE.Library.LE.Consul.Interfaces
{
    public interface IConsulServicesRegistry
    {
        Task<AgentService> GetAsync(string name);
        void SetupConsul(ConsulOptions consulOptions, IApplicationBuilder app);
        Task StopAsync(CancellationToken cancellationToken = default);
        Task StartAsync(CancellationToken cancellationToken = default);
    }
}
