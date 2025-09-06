using Learnova.Business.DTOs.PineconeDTO;

namespace Learnova.Business.Services.Interfaces.PdfProccessingInterfaces
{
    public interface IPineconeService
    {

        Task<PineconeUpsertResult> UpsertVectorsAsync(IEnumerable<PineconeVector> vectors, string? indexName = null);
        Task<PineconeIndexStats> GetIndexStatsAsync(string? indexName = null);
        Task<bool> HealthCheckAsync();

        Task<PineconeQueryResult> QueryByVectorAsync(float[] vector, int topK = 10, object? filter = null, bool includeMetadata = true, string? indexName = null);

    }

}



