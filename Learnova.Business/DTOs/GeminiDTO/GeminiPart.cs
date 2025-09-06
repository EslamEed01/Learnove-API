using System.Text.Json.Serialization;

namespace Learnova.Business.DTOs.GeminiDTO
{
    public class GeminiPart
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }
}
