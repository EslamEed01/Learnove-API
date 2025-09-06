using System.Text.Json.Serialization;

namespace Learnova.Business.DTOs.PineconeDTO
{
    public class PineconeQueryResult
    {
        [JsonPropertyName("matches")]
        public List<PineconeMatch> Matches { get; set; } = new();

        [JsonPropertyName("namespace")]
        public string? Namespace { get; set; }

        public bool Success { get; set; } = true;
        public string? ErrorMessage { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
