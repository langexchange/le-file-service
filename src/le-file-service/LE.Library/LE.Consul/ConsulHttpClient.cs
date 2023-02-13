using LE.Library.LE.Consul.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LE.Library.LE.Consul
{
    public class ConsulHttpClient : IConsulHttpClient
    {
        private HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;

        public ConsulHttpClient(HttpClient client, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _client = client;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<T> GetAsync<T>(string requestUri, Authorization authorization = default, CancellationToken cancellationToken = default)
        {
            var method = HttpMethod.Get;

            return await RequestAsync<T>(requestUri, method, authorization, cancellationToken);
        }

        public async Task<T> PostAsync<T>(string requestUri, HttpContent httpContent, Authorization authorization = default, CancellationToken cancellationToken = default)
        {
            var method = HttpMethod.Post;

            return await RequestAsync<T>(requestUri, method, authorization, cancellationToken, httpContent);
        }

        public async Task<T> PutAsync<T>(string requestUri, HttpContent httpContent, Authorization authorization = default, CancellationToken cancellationToken = default)
        {
            var method = HttpMethod.Put;

            return await RequestAsync<T>(requestUri, method, authorization, cancellationToken, httpContent);
        }

        public Task<T> Post<T>(string requestUri, HttpContent httpContent, Authorization authorization = default, CancellationToken cancellationToken = default)
        {
            var method = HttpMethod.Post;

            return Request<T>(requestUri, method, authorization, cancellationToken, httpContent);
        }

        public async Task<T> DeleteAsync<T>(string requestUri, Authorization authorization = default, CancellationToken cancellationToken = default)
        {
            var method = HttpMethod.Delete;

            return await RequestAsync<T>(requestUri, method, authorization, cancellationToken);
        }

        private async Task<T> RequestAsync<T>(string uri, HttpMethod method, Authorization authorization,
            CancellationToken cancellationToken, HttpContent httpContent = null)
        {
            try
            {
                var message = new HttpRequestMessage(method, uri)
                  .CopyAuthorizationHeaderFrom(_httpContextAccessor?.HttpContext)
                  .CopyCountryCodeHeaderFrom(_httpContextAccessor?.HttpContext)
                  .Apply(authorization);

                if (httpContent != null)
                {
                    message.Content = httpContent;
                }

                var response = await _client.SendAsync(message, cancellationToken);

                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new HttpRequestException(await response.Content.ReadAsStringAsync());
                }

                var content = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception ex)
            {
                throw new Exception($"{uri} {ex.ToString()}");
            }
        }

        private Task<T> Request<T>(string requestUrl, HttpMethod method, Authorization authorization,
            CancellationToken cancellationToken, HttpContent httpContent = null)
        {
            _client = new HttpClient();
            var uri = requestUrl.StartsWith("http://") ? requestUrl : $"http://{requestUrl}";

            var message = new HttpRequestMessage(method, uri)
                .CopyAuthorizationHeaderFrom(_httpContextAccessor.HttpContext)
                .Apply(authorization);

            if (httpContent != null)
            {
                message.Content = httpContent;
            }

            var response = _client.SendAsync(message, cancellationToken);

            if (response.Result.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }

            var content = response.Result.Content.ReadAsStringAsync();

            return Task.FromResult(JsonConvert.DeserializeObject<T>(content.Result));
        }
    }
}
