using System.ComponentModel.DataAnnotations;

namespace Learnova.Business.DTOs.EmbeddingDTO
{
    public class CohereOptions
    {
        public const string SectionName = "Cohere";

        [Required]
        public string ApiKey { get; set; } = string.Empty;

        public string BaseUrl { get; set; } = "https://api.cohere.ai/v1";
        public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromMinutes(2);
        public string DefaultModel { get; set; } = "embed-english-v3.0";
        public int MaxRetries { get; set; } = 3;
        public int BatchSize { get; set; } = 96;
    }
}