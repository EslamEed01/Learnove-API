using Learnova.Business.Implementations.PdfProccessingServices;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Learnova.API.HealthChecks
{
    public class TextChunkerHealthCheck : IHealthCheck
    {
        private readonly TextChunker _textChunker;

        public TextChunkerHealthCheck(TextChunker textChunker)
        {
            _textChunker = textChunker;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Test basic chunking functionality
                var testPages = new List<(int pageNumber, string text)>
                {
                    (1, "This is a test document with some content that should be chunked properly. " +
                       "It contains multiple sentences to test the chunking algorithm.")
                };

                var chunks = _textChunker.SplitIntoChunks(testPages, Guid.NewGuid(), 100, 20);

                if (chunks.Any() && chunks.All(c => !string.IsNullOrEmpty(c.TextContent)))
                {
                    return Task.FromResult(HealthCheckResult.Healthy($"Text chunker is working. Generated {chunks.Count} chunks from test data"));
                }

                return Task.FromResult(HealthCheckResult.Degraded("Text chunker generated empty or invalid chunks"));
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy($"Text chunker health check failed: {ex.Message}"));
            }
        }
    }
}