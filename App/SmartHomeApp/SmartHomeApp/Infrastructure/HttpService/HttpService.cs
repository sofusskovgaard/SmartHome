using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using SmartHomeApp.Dtos.DataApiService;
using SmartHomeApp.Infrastructure.TinyIoC;

namespace SmartHomeApp.Infrastructure.HttpService
{
    public class HttpService : IHttpService
    {
        private static readonly HttpClient _client = new HttpClient(
#if DEBUG
        new HttpClientHandler { ServerCertificateCustomValidationCallback = (_, __, ___, ____) => true  }
#endif
        );

        public async Task<T> GetAsync<T>(string path, string bearerToken = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            _configureClient(bearerToken);

            var responseMessage = await _client.GetAsync(path, cancellationToken: cancellationToken);

            if (!responseMessage.IsSuccessStatusCode)
            {
                throw new HttpRequestException(responseMessage.ReasonPhrase);
            }

            var json = await responseMessage.Content.ReadAsStreamAsync();
            
            return await JsonSerializer.DeserializeAsync<T>(json, cancellationToken: cancellationToken);
        }

        public async Task<bool> PostAsync(string path, string bearerToken = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            _configureClient(bearerToken);
            var responseMessage = await _client.PostAsync(path, null, cancellationToken: cancellationToken);
            return responseMessage.IsSuccessStatusCode;
        }

        private void _configureClient(string bearerToken = null)
        {
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrEmpty(bearerToken))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }
            else
            {
                _client.DefaultRequestHeaders.Authorization = null;
            }
        }
    }
}