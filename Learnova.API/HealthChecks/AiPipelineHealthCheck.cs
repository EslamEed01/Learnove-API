using Learnova.Business.Services.Interfaces.PdfProccessingInterfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Learnova.API.HealthChecks
{
    public class AiPipelineHealthCheck : IHealthCheck
    {
        private readonly IEmbeddingService _embeddingService;
        private readonly IPineconeService _pineconeService;
        private readonly IGeminiService _geminiService;

        public AiPipelineHealthCheck(
            IEmbeddingService embeddingService,
            IPineconeService pineconeService,
            IGeminiService geminiService)
        {
            _embeddingService = embeddingService;
            _pineconeService = pineconeService;
            _geminiService = geminiService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var issues = new List<string>();

                try
                {
                    await _embeddingService.ValidateApiKeyAsync();
                }
                catch
                {
                    issues.Add("Embedding service unavailable");
                }

                try
                {
                    await _pineconeService.HealthCheckAsync();
                }
                catch
                {
                    issues.Add("Vector store unavailable");
                }

                try
                {
                    await _geminiService.GenerateTextAsync("test");
                }
                catch
                {
                    issues.Add("AI generation unavailable");
                }

                if (!issues.Any())
                {
                    return HealthCheckResult.Healthy("Complete AI pipeline is functional");
                }
                else if (issues.Count == 1)
                {
                    return HealthCheckResult.Degraded($"AI pipeline partially functional. Issues: {string.Join(", ", issues)}");
                }
                else
                {
                    return HealthCheckResult.Unhealthy($"AI pipeline has multiple issues: {string.Join(", ", issues)}");
                }
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"AI pipeline health check failed: {ex.Message}");
            }
        }
    }
}