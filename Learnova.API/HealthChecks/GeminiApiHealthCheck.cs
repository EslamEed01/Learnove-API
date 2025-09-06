using Learnova.Business.Services.Interfaces.PdfProccessingInterfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Learnova.API.HealthChecks
{
    public class GeminiApiHealthCheck : IHealthCheck
    {
        private readonly IGeminiService _geminiService;

        public GeminiApiHealthCheck(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var testResponse = await _geminiService.GenerateTextAsync("Health check test");

                if (!string.IsNullOrEmpty(testResponse))
                {
                    return HealthCheckResult.Healthy("Gemini API is responding correctly");
                }

                return HealthCheckResult.Degraded("Gemini API responded but with empty content");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"Gemini API health check failed: {ex.Message}");
            }
        }
    }
}