using System.ComponentModel.DataAnnotations;

namespace Learnova.Business.DTOs.PineconeDTO
{
    public class PineconeOptions
    {
        public const string SectionName = "Pinecone";

        [Required]
        public string ApiKey { get; set; } = string.Empty;

        [Required]
        public string IndexName { get; set; } = string.Empty;

        public string Environment { get; set; } = "us-east-1";
        public string BaseUrl { get; set; } = "https://api.pinecone.io";
        public int BatchSize { get; set; } = 100;
        public int MaxRetries { get; set; } = 3;
        public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);
        public int Dimension { get; set; } = 1536;
        public string Metric { get; set; } = "cosine";
    }
}