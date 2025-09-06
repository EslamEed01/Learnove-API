using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learnova.Domain.Entities
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string? Title { get; set; }

        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string? Comment { get; set; }

        public DateTime ReviewDate { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public AppUser User { get; set; }

        [JsonIgnore] // This breaks the cycle
        public Course Course { get; set; }
    }
}