using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SmartHomeApp.Dtos.DataApiService;

namespace SmartHomeApp.Services.DataApiService
{
    public interface IDataApiService
    {
        public Task<DataPointDto> GetLatest(int days, MeasurementType measurementType, LocationType locationType);
        public Task<DataPointDto> GetLatest(int days, MeasurementType measurementType, LocationType locationType, CancellationToken cancellationToken);
        public Task<IEnumerable<DataPointDto>> GetLastDays(int days, MeasurementType measurementType, LocationType locationType);
        public Task<IEnumerable<DataPointDto>> GetLastDays(int days, MeasurementType measurementType, LocationType locationType, CancellationToken cancellationToken);
        public Task<IEnumerable<DataPointDto>> GetLastHours(int hours, MeasurementType measurementType, LocationType locationType);
        public Task<IEnumerable<DataPointDto>> GetLastHours(int hours, MeasurementType measurementType, LocationType locationType, CancellationToken cancellationToken);
        public Task<IEnumerable<DataPointDto>> GetLastMinutes(int minutes, MeasurementType measurementType, LocationType locationType);
        public Task<IEnumerable<DataPointDto>> GetLastMinutes(int minutes, MeasurementType measurementType, LocationType locationType, CancellationToken cancellationToken);
    }
}