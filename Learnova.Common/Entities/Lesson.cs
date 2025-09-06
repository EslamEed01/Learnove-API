using System.ComponentModel.DataAnnotations;

namespace Learnova.Domain.Entities
{
    public class Lesson
    {

        [Key]
        public int LessonId { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int OrderIndex { get; set; }

        // Navigation
        public Course Course { get; set; }
        public ICollection<LessonVideo> Videos { get; set; }
    }
}
