namespace Learnova.Business.DTOs.AiDTO
{
    public class AIQueryResponse
    {
        public bool Success { get; set; }
        public string Answer { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public AIQueryMetadata Metadata { get; set; } = new();
        public List<RetrievedChunk> RetrievedChunks { get; set; } = new();
        public TimeSpan ProcessingTime { get; set; }
    }

}
