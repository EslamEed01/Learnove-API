using System.Text.Json.Serialization;

namespace Learnova.Business.DTOs.PineconeDTO
{
    public class PineconeVector
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("values")]
        public float[] Values { get; set; } = Array.Empty<float>();

        [JsonPropertyName("metadata")]
        public Dictionary<string, object>? Metadata { get; set; }

    }

}
