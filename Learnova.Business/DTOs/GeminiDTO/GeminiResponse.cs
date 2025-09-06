using System.Text.Json.Serialization;

namespace Learnova.Business.DTOs.GeminiDTO
{
    public class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public List<GeminiSimpleCandidate> Candidates { get; set; } = new();
    }


}