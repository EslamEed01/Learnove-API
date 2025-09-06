namespace Learnova.Business.DTOs.PdfProcessingDTO
{
    public class PdfMetadata
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Subject { get; set; }
        public string? Creator { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int PageCount { get; set; }
    }
}
