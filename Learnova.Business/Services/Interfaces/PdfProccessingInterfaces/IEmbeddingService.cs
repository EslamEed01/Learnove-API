using Learnova.Business.DTOs.EmbeddingDTO;

namespace Learnova.Business.Services.Interfaces.PdfProccessingInterfaces
{
    public interface IEmbeddingService
    {
        Task<EmbeddingResult> CreateEmbeddingAsync(string text, EmbeddingOptions? options = null);
        Task<BatchEmbeddingResult> CreateEmbeddingsAsync(IEnumerable<string> texts, EmbeddingOptions? options = null);
        Task<EmbeddingServiceStats> GetStatsAsync();
        Task<bool> ValidateApiKeyAsync();
    }
}