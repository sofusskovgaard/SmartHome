using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHomeApp.Infrastructure.HttpService
{
    public interface IHttpService
    {
        public Task<T> GetAsync<T>(string path, string bearerToken = null, CancellationToken cancellationToken = default(CancellationToken));

        // public Task<bool> GetAsync(string path, string bearerToken = null, CancellationToken cancellationToken = default(CancellationToken));

        public Task<bool> PostAsync(string path, string bearerToken = null, CancellationToken cancellationToken = default(CancellationToken));
        
        // public Task<R> PostAsync<T, R>(Uri path, T data);
    }
}