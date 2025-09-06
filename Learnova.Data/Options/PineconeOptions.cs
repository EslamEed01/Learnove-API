namespace Learnova.Infrastructure.Options
{
    public class PineconeOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string IndexName { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public int BatchSize { get; set; } = 100;
        public int MaxRetries { get; set; } = 3;
        public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);
        public int Dimension { get; set; } = 1536;
        public string Metric { get; set; } = "cosine";
    }
}
