using System.Text.Json.Serialization;

namespace AquaClient.Classes.Responses
{
    public partial class AquaDeviceInfo
    {
        [JsonPropertyName("id")]
        public int DeviceId { get; set; }
        [JsonPropertyName("name")]
        public string? DeviceName { get; set; }
        [JsonPropertyName("type")]
        public string? DeviceType { get; set; }
        [JsonPropertyName("status")]
        public int? DeviceStatus { get; set; }
    }
}
