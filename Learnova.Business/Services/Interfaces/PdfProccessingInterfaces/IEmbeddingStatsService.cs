using Learnova.Business.DTOs.EmbeddingDTO;

namespace Learnova.Business.Services.Interfaces.PdfProccessingInterfaces
{
    public interface IEmbeddingStatsService
    {
        void RecordRequest(int tokenCount, bool fromCache, double durationMs);
        void RecordError(string error);
        EmbeddingServiceStats GetStatsSnapshot();
    }
}