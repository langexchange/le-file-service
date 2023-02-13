using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace LE.Library.LE.Consul.Interfaces
{
    public interface IConsulHttpClient
    {
        Task<T> GetAsync<T>(string requestUri, Authorization authorization = default, CancellationToken cancellationToken = default);

        Task<T> PostAsync<T>(string requestUri, System.Net.Http.HttpContent content, Authorization authorization = default, CancellationToken cancellationToken = default);

        Task<T> PutAsync<T>(string requestUri, System.Net.Http.HttpContent httpContent, Authorization authorization = default, CancellationToken cancellationToken = default);

        Task<T> DeleteAsync<T>(string requestUri, Authorization authorization = default, CancellationToken cancellationToken = default);

        Task<T> Post<T>(string requestUri, System.Net.Http.HttpContent httpContent, Authorization authorization = default, CancellationToken cancellationToken = default);
    }
}
