using Learnova.Business.DTOs.PdfProcessingDTO;

namespace Learnova.Business.Services.Interfaces
{
    public interface IBackgroundJobService
    {
        Task SendWelcomeEmailAsync(string userId, string email, string firstName);
        Task ProcessPdfContentAsync(Guid pdfContentId, string s3Bucket, string s3Key, PdfProcessingOptions? options = null);


    }
}