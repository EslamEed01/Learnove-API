using System.ComponentModel.DataAnnotations;

namespace Learnova.Domain.Entities
{
    public class PdfChunk
    {
        [Key]
        public Guid Id { get; set; }
        public Guid PdfContentId { get; set; }
        public int ChunkIndex { get; set; }

        [Required]
        public string PageNumbers { get; set; }

        [Required]
        public string TextContent { get; set; }

        public string? PineconeVectorId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // navigation properties
        public pdfContents PdfContent { get; set; }
    }
}
