using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using SmartHomeApp.Dtos.DataApiService;
using SmartHomeApp.Infrastructure.HttpService;
using SmartHomeApp.Infrastructure.TinyIoC;
using SmartHomeApp.Services.AuthenticationService;
using Xamarin.Forms;

namespace SmartHomeApp.Services.DataApiService
{
    public class DataApiService : IDataApiService
    {
        private readonly IHttpService _httpService;

        private readonly IAuthenticationService _authenticationService;

        private readonly AsyncCachePolicy<IEnumerable<DataPointDto>> _cache;
        
        public DataApiService()
        {
            var container = TinyIoCContainer.Current;

            _cache = Policy.CacheAsync(
                container.Resolve<MemoryCacheProvider>("cache").AsyncFor<IEnumerable<DataPointDto>>(), TimeSpan.FromSeconds(30));
            _httpService = container.Resolve<IHttpService>();
            _authenticationService = container.Resolve<IAuthenticationService>();
        }

        public Task<DataPointDto> GetLatest(int days, MeasurementType measurementType, LocationType locationType)
        {
            return GetLatest(days, measurementType, locationType, CancellationToken.None);
        }

        public async Task<DataPointDto> GetLatest(int days, MeasurementType measurementType, LocationType locationType, CancellationToken cancellationToken)
        {
            var measurement = measurementType.ToString().ToLower();
            var location = locationType.ToString().ToLower();
            var operationKey = $"latest-{days}-days-{measurement}-{location}";

            var response = await _cache.ExecuteAsync((context, token) =>
            {
                StringBuilder path = new StringBuilder("http://localhost:4001/api/data");

                path.AppendFormat("/{0}", context[nameof(measurement)]);
                path.AppendFormat("/{0}", context[nameof(location)]);
                path.Append("/latest");
                path.AppendFormat("?offset_days={0}", days);

                return _httpService.GetAsync<IEnumerable<DataPointDto>>(path.ToString(), _authenticationService.Tokens[AuthenticationService.AuthenticationService.ACCESS_TOKEN], cancellationToken: token);
            }, new Context(operationKey, new Dictionary<string, object> { { nameof(measurement), measurement }, { nameof(location), location } }), cancellationToken);

            return response.FirstOrDefault();
        }

        public Task<IEnumerable<DataPointDto>> GetLastDays(int days, MeasurementType measurementType, LocationType locationType)
        {
            return GetLastDays(days, measurementType, locationType, CancellationToken.None);
        }
        
        public Task<IEnumerable<DataPointDto>> GetLastDays(int days, MeasurementType measurementType, LocationType locationType, CancellationToken cancellationToken)
        {
            var measurement = measurementType.ToString().ToLower();
            var location = locationType.ToString().ToLower();
            var operationKey = $"last-{days}-days-{measurement}-{location}";

            return _cache.ExecuteAsync((context, token) =>
            {
                StringBuilder path = new StringBuilder("http://localhost:4001/api/data");

                path.AppendFormat("/{0}", context[nameof(measurement)]);
                path.AppendFormat("/{0}", context[nameof(location)]);
                path.AppendFormat("?offset_days={0}", days);

                return _httpService.GetAsync<IEnumerable<DataPointDto>>(path.ToString(), _authenticationService.Tokens[AuthenticationService.AuthenticationService.ACCESS_TOKEN], cancellationToken: token);
            }, new Context(operationKey, new Dictionary<string, object> { { nameof(measurement), measurement }, { nameof(location), location } }), cancellationToken);
        }

        public Task<IEnumerable<DataPointDto>> GetLastHours(int hours, MeasurementType measurementType, LocationType locationType)
        {
            return GetLastHours(hours, measurementType, locationType, CancellationToken.None);
        }
        
        public Task<IEnumerable<DataPointDto>> GetLastHours(int hours, MeasurementType measurementType, LocationType locationType, CancellationToken cancellationToken)
        {
            var measurement = measurementType.ToString().ToLower();
            var location = locationType.ToString().ToLower();
            var operationKey = $"last-{hours}-hours-{measurement}-{location}";

            return _cache.ExecuteAsync((context, token) =>
            {
                StringBuilder path = new StringBuilder("http://localhost:4001/api/data");

                path.AppendFormat("/{0}", context[nameof(measurement)]);
                path.AppendFormat("/{0}", context[nameof(location)]);
                path.AppendFormat("?offset_hours={0}", hours);

                return _httpService.GetAsync<IEnumerable<DataPointDto>>(path.ToString(), _authenticationService.Tokens[AuthenticationService.AuthenticationService.ACCESS_TOKEN], cancellationToken: token);
            }, new Context(operationKey, new Dictionary<string, object> { { nameof(measurement), measurement }, { nameof(location), location } }), cancellationToken);
        }
        
        public Task<IEnumerable<DataPointDto>> GetLastMinutes(int minutes, MeasurementType measurementType, LocationType locationType)
        {
            return GetLastMinutes(minutes, measurementType, locationType, CancellationToken.None);
        }

        public Task<IEnumerable<DataPointDto>> GetLastMinutes(int minutes, MeasurementType measurementType, LocationType locationType, CancellationToken cancellationToken)
        {
            var measurement = measurementType.ToString().ToLower();
            var location = locationType.ToString().ToLower();
            var operationKey = $"last-{minutes}-minutes-{measurement}-{location}";

            return _cache.ExecuteAsync((context, token) =>
            {
                StringBuilder path = new StringBuilder("http://localhost:4001/api/data");

                path.AppendFormat("/{0}", context[nameof(measurement)]);
                path.AppendFormat("/{0}", context[nameof(location)]);
                path.AppendFormat("?offset_minutes={0}", minutes);

                return _httpService.GetAsync<IEnumerable<DataPointDto>>(path.ToString(), _authenticationService.Tokens[AuthenticationService.AuthenticationService.ACCESS_TOKEN], cancellationToken: token);
            }, new Context(operationKey, new Dictionary<string, object> { { nameof(measurement), measurement }, { nameof(location), location } }), cancellationToken);
        }
    }
}