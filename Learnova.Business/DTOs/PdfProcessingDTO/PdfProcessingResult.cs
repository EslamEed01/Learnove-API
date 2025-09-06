namespace Learnova.Business.DTOs.PdfProcessingDTO
{
    public class PdfProcessingResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public int TotalPages { get; set; }
        public int TotalChunks { get; set; }
        public TimeSpan ProcessingTime { get; set; }
        public long FileSizeBytes { get; set; }
        public PdfMetadata? Metadata { get; set; }



    }
}
