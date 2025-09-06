namespace Learnova.Business.DTOs.EmbeddingDTO
{
    public class EmbeddingServiceStats
    {
        public int TotalRequests { get; set; }
        public int CacheMisses { get; set; }
        public int CacheHits { get; set; }
        public double AverageResponseTimeMs { get; set; }
        public int TotalTokensProcessed { get; set; }
        public DateTime LastRequestTime { get; set; }
        public double CacheHitRate => TotalRequests > 0 ? (double)CacheHits / TotalRequests : 0;
        public List<string> RecentErrors { get; set; } = new();
    }
}