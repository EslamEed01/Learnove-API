namespace Learnova.Business.DTOs.EmbeddingDTO
{
    public class BatchEmbeddingResult
    {
        public List<EmbeddingResult> Results { get; set; } = new();
        public int TotalProcessed { get; set; }
        public int FailureCount { get; set; }
        public int SuccessCount { get; set; }
        public double TotalDurationMs { get; set; }
        public bool Success => FailureCount == 0;
        public List<string> Errors { get; set; } = new();
    }
}