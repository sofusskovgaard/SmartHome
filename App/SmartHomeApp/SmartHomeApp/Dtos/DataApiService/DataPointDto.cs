using System.Text.Json.Serialization;

namespace SmartHomeApp.Dtos.DataApiService
{
    public class DataPointDto
    {
        [JsonPropertyName("_field")]
        public string Field { get; set; }
        
        [JsonPropertyName("_measurement")]
        public string Measurement { get; set; }

        [JsonPropertyName("_time")]
        public ulong Time { get; set; }
        
        [JsonPropertyName("_value")]
        public float Value { get; set; }
        
        public string Location { get; set; }
    }
}