using System.ComponentModel.DataAnnotations;

namespace Learnova.Business.DTOs.GeminiDTO
{
    public class GeminiOptions
    {
        public const string SectionName = "Gemini";

        [Required]
        public string ApiKey { get; set; } = string.Empty;

        public string BaseUrl { get; set; } = "https://generativelanguage.googleapis.com";
        public string DefaultModel { get; set; } = "gemini-1.5-flash";
        public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);
        public int MaxRetries { get; set; } = 3;
        public double Temperature { get; set; } = 0.7;
        public int MaxTokens { get; set; } = 2048;
        public double TopP { get; set; } = 0.8;
        public int TopK { get; set; } = 40;
    }
}