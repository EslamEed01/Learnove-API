using Learnova.Business.DTOs.EmbeddingDTO;
using Learnova.Business.Services.Interfaces.PdfProccessingInterfaces;

namespace Learnova.Business.Implementations.PdfProccessingServices
{
    public class EmbeddingStatsService : IEmbeddingStatsService
    {
        private readonly object _lock = new();
        private EmbeddingServiceStats _stats = new();
        private readonly List<double> _responseTimes = new();
        private const int MaxRecentErrors = 10;

        public void RecordRequest(int tokenCount, bool fromCache, double durationMs)
        {
            lock (_lock)
            {
                _stats.TotalRequests++;
                _stats.TotalTokensProcessed += tokenCount;
                _stats.LastRequestTime = DateTime.UtcNow;

                if (fromCache)
                {
                    _stats.CacheHits++;
                }
                else
                {
                    _stats.CacheMisses++;
                }

                _responseTimes.Add(durationMs);

                if (_responseTimes.Count > 1000)
                {
                    _responseTimes.RemoveRange(0, 500);
                }

                _stats.AverageResponseTimeMs = _responseTimes.Average();
            }
        }

        public EmbeddingServiceStats GetStatsSnapshot()
        {
            lock (_lock)
            {
                return new EmbeddingServiceStats
                {
                    TotalRequests = _stats.TotalRequests,
                    CacheHits = _stats.CacheHits,
                    CacheMisses = _stats.CacheMisses,
                    AverageResponseTimeMs = _stats.AverageResponseTimeMs,
                    TotalTokensProcessed = _stats.TotalTokensProcessed,
                    LastRequestTime = _stats.LastRequestTime,
                    RecentErrors = new List<string>(_stats.RecentErrors)
                };
            }
        }

        public void RecordError(string error)
        {
            lock (_lock)
            {
                _stats.RecentErrors.Add($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}: {error}");

                if (_stats.RecentErrors.Count > MaxRecentErrors)
                {
                    _stats.RecentErrors.RemoveAt(0);
                }
            }
        }
    }
}