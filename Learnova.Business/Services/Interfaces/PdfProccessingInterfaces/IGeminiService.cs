using Learnova.Business.DTOs.AiDTO;

namespace Learnova.Business.Services.Interfaces.PdfProccessingInterfaces
{
    public interface IGeminiService
    {
        Task<string> GenerateAnswerAsync(string question, List<RetrievedChunk> chunks);
        Task<string> GenerateTextAsync(string prompt);
    }
}