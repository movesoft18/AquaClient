using System.Text.Json.Serialization;

namespace AquaClient.Classes.Responses
{
    public class User
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("surname")]
        public string? Surname { get; set; }
        [JsonPropertyName("firstname")]
        public string Firstname { get; set; }
        [JsonPropertyName("middlename")]
        public string? Middlename { get; set; }
        [JsonPropertyName("phone")]
        public string Phone { get; set; }
        [JsonPropertyName("email")]
        public string? Email { get; set; }
    }
}
