namespace Learnova.Business.DTOs.AiDTO
{
    public class RetrievedChunk
    {
        public Guid ChunkId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string PageNumbers { get; set; } = string.Empty;
        public float SimilarityScore { get; set; }
        public int ChunkIndex { get; set; }
        public Guid PdfContentId { get; set; }
    }

}
