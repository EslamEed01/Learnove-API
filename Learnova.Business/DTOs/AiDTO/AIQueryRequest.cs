using Learnova.Domain.Enums;

namespace Learnova.Business.DTOs.AiDTO
{
    public class AIQueryRequest
    {
        public string Question { get; set; } = string.Empty;
        public Guid? PdfContentId { get; set; }
        public double SimilarityThreshold { get; set; } = 0.7;
        public int MaxChunks { get; set; } = 5;
        public AIQueryType QueryType { get; set; } = AIQueryType.Question;
    }

}
