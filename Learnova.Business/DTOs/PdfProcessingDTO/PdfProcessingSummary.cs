namespace Learnova.Business.DTOs.PdfProcessingDTO
{
    public class PdfProcessingSummary
    {
        public int TotalChunks { get; set; }
        public int TotalPages { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
