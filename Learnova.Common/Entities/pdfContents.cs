using System.ComponentModel.DataAnnotations;

namespace Learnova.Domain.Entities
{
    public class pdfContents
    {

        [Key]
        public Guid Id { get; set; }
        public int CourseId { get; set; }
        public string FileName { get; set; }
        public string? Description { get; set; }

        public string FileUrl { get; set; }
        public long FileSize { get; set; }
        public string S3FileUrl { get; set; }
        public string UploadedById { get; set; }
        public int? TotalPages { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


        // navigation properties
        public Course Course { get; set; }
        public AppUser UploadedBy { get; set; }
        public ICollection<PdfChunk> Chunks { get; set; }





    }
}
