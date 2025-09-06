namespace Learnova.Business.DTOs.PdfProcessingDTO
{
    public class PdfProcessingOptions
    {
        public int ChunkSize { get; set; } = 1000;
        public int OverlapSize { get; set; } = 100;
        public bool ExtractImages { get; set; } = false;
        public bool ExtractMetadata { get; set; } = true;
        public int BatchSize { get; set; } = 50;
    }
}
