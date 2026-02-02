using System.Text.Json.Serialization;

namespace AquaClient.Classes.Responses
{
    public class ConfirmCodeDataResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
        [JsonPropertyName("user_id")]
        public int UserId { get; set; }
    }
}
