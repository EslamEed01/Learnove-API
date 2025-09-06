using System.ComponentModel.DataAnnotations;

namespace Learnova.Domain.Entities
{
    public class Course
    {

        [Key]
        public int CourseId { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? LessonsCount { get; set; }



        // Navigation prop
        public Category Category { get; set; } // done
        public AppUser Creator { get; set; } // done
        public ICollection<Lesson> Lessons { get; set; } // done
        public ICollection<Enrollment> Enrollments { get; set; } // done
        public ICollection<pdfContents> PdfContents { get; set; }
        public ICollection<LessonVideo> lessonVideos { get; set; }
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

    }
}
