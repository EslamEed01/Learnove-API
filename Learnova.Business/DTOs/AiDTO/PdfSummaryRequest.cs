using Learnova.Domain.Enums;

namespace Learnova.Business.DTOs.AiDTO
{
    public class PdfSummaryRequest
    {
        public Guid PdfContentId { get; set; }
        public SummaryType Type { get; set; } = SummaryType.Overview;
        public int MaxChunks { get; set; } = 8;
        public SummaryLength Length { get; set; } = SummaryLength.Medium;
    }

}
