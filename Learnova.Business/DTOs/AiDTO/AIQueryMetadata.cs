namespace Learnova.Business.DTOs.AiDTO
{
    public class AIQueryMetadata
    {
        public int TotalChunksFound { get; set; }
        public int ChunksUsed { get; set; }
        public string SearchMethod { get; set; } = string.Empty;
        public float HighestSimilarity { get; set; }

    }

}
