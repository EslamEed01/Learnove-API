using System.Text.Json.Serialization;

namespace Learnova.Business.DTOs.GeminiDTO
{
    public class GeminiGenerationConfig
    {
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; } = 0.7;

        [JsonPropertyName("topK")]
        public int TopK { get; set; } = 40;

        [JsonPropertyName("topP")]
        public double TopP { get; set; } = 0.8;

        [JsonPropertyName("maxOutputTokens")]
        public int MaxOutputTokens { get; set; } = 2048;

        [JsonPropertyName("stopSequences")]
        public List<string>? StopSequences { get; set; }
    }
}
