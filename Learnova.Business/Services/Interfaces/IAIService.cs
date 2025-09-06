using Learnova.Business.DTOs.AiDTO;

namespace Learnova.Business.Services.Interfaces
{
    public interface IAIService
    {
        Task<AIQueryResponse> AskQuestionAsync(AIQueryRequest request);
        Task<AIQueryResponse> SummarizePdfAsync(PdfSummaryRequest request);
        Task<List<RetrievedChunk>> SearchSimilarChunksAsync(string query, Guid? pdfContentId = null, int maxResults = 5);
    }

}
