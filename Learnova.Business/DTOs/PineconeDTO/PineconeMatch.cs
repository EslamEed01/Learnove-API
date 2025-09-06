using System.Text.Json;
using System.Text.Json.Serialization;

namespace Learnova.Business.DTOs.PineconeDTO
{
    public class PineconeMatch
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("score")]
        public float Score { get; set; }

        [JsonPropertyName("metadata")]
        public JsonElement Metadata { get; set; }
    }
}
