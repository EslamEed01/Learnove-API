using Hangfire;
using Learnova.Business.DTOs.PdfProcessingDTO;
using Learnova.Business.Services.Interfaces;
using Learnova.Business.Services.Interfaces.PdfProccessingServices;
using Learnova.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;

namespace Learnova.Business.Implementations
{
    public class BackgroundJobService : IBackgroundJobService
    {
        private readonly ILogger<BackgroundJobService> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IPdfProcessingService _pdfProcessingService;
        private readonly IOrderRepository _orderRepository;
        private readonly ICourseRepository _courseRepository;

        public BackgroundJobService(
            ILogger<BackgroundJobService> logger,
            IEmailSender emailSender,
            IPdfProcessingService pdfProcessingService,
            IOrderRepository orderRepository,
            ICourseRepository courseRepository)
        {
            _logger = logger;
            _emailSender = emailSender;
            _pdfProcessingService = pdfProcessingService;
            _orderRepository = orderRepository;
            _courseRepository = courseRepository;
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task SendWelcomeEmailAsync(string userId, string email, string firstName)
        {
            try
            {
                _logger.LogInformation("Sending welcome email to user {UserId}", userId);

                var subject = "Welcome to Learnova!";
                var body = $"Hello {firstName},\n\nWelcome to Learnova! We're excited to have you on our learning platform.";

                await _emailSender.SendEmailAsync(email, subject, body);

                _logger.LogInformation("Welcome email sent successfully to user {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send welcome email to user {UserId}", userId);
                throw;
            }
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task ProcessPdfContentAsync(Guid pdfContentId, string s3Bucket, string s3Key, PdfProcessingOptions? options = null)
        {
            try
            {
                _logger.LogInformation("Processing PDF content {PdfContentId}", pdfContentId);

                await _pdfProcessingService.ProcessPdfAsync(pdfContentId, s3Bucket, s3Key, options);

                _logger.LogInformation("PDF content {PdfContentId} processed successfully", pdfContentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process PDF content {PdfContentId}", pdfContentId);
                throw;
            }
        }


    }
}