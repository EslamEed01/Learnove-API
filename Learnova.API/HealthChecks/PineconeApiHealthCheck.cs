using Learnova.Business.Services.Interfaces.PdfProccessingInterfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Learnova.API.HealthChecks
{
    public class PineconeApiHealthCheck : IHealthCheck
    {
        private readonly IPineconeService _pineconeService;

        public PineconeApiHealthCheck(IPineconeService pineconeService)
        {
            _pineconeService = pineconeService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var isHealthy = await _pineconeService.HealthCheckAsync();
                if (isHealthy)
                {
                    var indexStats = await _pineconeService.GetIndexStatsAsync();
                    return HealthCheckResult.Healthy($"Pinecone API is healthy. Total vectors: {indexStats.TotalVectorCount}");
                }
                return HealthCheckResult.Degraded("Pinecone API responded but may have issues");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"Pinecone API health check failed: {ex.Message}");
            }
        }
    }
}