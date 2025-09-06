namespace Learnova.Business.DTOs.EmbeddingDTO
{
    public class EmbeddingResult
    {
        public float[] Embedding { get; set; } = Array.Empty<float>();
        public int TokenCount { get; set; }
        public string Model { get; set; } = string.Empty;
        public bool IsFromCache { get; set; }
        public double RequestDurationMs { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}