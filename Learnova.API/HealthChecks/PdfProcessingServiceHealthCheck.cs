using Learnova.Business.Services.Interfaces.PdfProccessingServices;
using Learnova.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Learnova.API.HealthChecks
{
    public class PdfProcessingServiceHealthCheck : IHealthCheck
    {
        private readonly IPdfProcessingService _pdfProcessingService;
        private readonly LearnoveDbContext _dbContext;

        public PdfProcessingServiceHealthCheck(IPdfProcessingService pdfProcessingService, LearnoveDbContext dbContext)
        {
            _pdfProcessingService = pdfProcessingService;
            _dbContext = dbContext;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var chunksCount = await _dbContext.pdfChunks.CountAsync(cancellationToken);

                return HealthCheckResult.Healthy($"PDF processing service is accessible. Total chunks in database: {chunksCount}");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"PDF processing service health check failed: {ex.Message}");
            }
        }
    }
}