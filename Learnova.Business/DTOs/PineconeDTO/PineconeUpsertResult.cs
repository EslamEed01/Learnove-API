using System.Text.Json.Serialization;

namespace Learnova.Business.DTOs.PineconeDTO
{
    public class PineconeUpsertResult
    {
        [JsonPropertyName("upsertedCount")]
        public int UpsertedCount { get; set; }

        public bool Success { get; set; } = true;
        public string? ErrorMessage { get; set; }
        public TimeSpan Duration { get; set; }
        public List<string> ProcessedIds { get; set; } = new();
    }
}
