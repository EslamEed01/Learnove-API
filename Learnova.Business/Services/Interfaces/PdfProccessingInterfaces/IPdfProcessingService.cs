using Learnova.Business.DTOs.PdfProcessingDTO;

namespace Learnova.Business.Services.Interfaces.PdfProccessingServices
{
    public interface IPdfProcessingService
    {
        Task<PdfProcessingResult> ProcessPdfAsync(Guid pdfContentId, string s3Bucket, string s3Key, PdfProcessingOptions? options = null);
        Task<bool> DeletePdfChunksAsync(Guid pdfContentId);
        Task<PdfProcessingSummary> GetProcessingSummaryAsync(Guid pdfContentId);
    }
}
