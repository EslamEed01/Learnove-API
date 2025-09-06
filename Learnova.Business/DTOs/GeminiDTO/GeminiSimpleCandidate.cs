using System.Text.Json.Serialization;

namespace Learnova.Business.DTOs.GeminiDTO
{
    public class GeminiSimpleCandidate
    {
        [JsonPropertyName("content")]
        public GeminiContent Content { get; set; } = new();

        [JsonPropertyName("finishReason")]
        public string? FinishReason { get; set; }

        [JsonPropertyName("index")]
        public int Index { get; set; }
    }
}
