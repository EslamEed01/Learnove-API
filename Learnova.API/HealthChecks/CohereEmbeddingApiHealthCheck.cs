using Learnova.Business.Services.Interfaces.PdfProccessingInterfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Learnova.API.HealthChecks
{
    public class CohereEmbeddingApiHealthCheck : IHealthCheck
    {
        private readonly IEmbeddingService _embeddingService;

        public CohereEmbeddingApiHealthCheck(IEmbeddingService embeddingService)
        {
            _embeddingService = embeddingService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var isValid = await _embeddingService.ValidateApiKeyAsync();

                if (isValid)
                {
                    var stats = await _embeddingService.GetStatsAsync();
                    return HealthCheckResult.Healthy($"Cohere Embedding API is healthy. Total requests: {stats.TotalRequests}");
                }

                return HealthCheckResult.Unhealthy("Cohere Embedding API key validation failed");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"Cohere Embedding API health check failed: {ex.Message}");
            }
        }
    }
}