using System.ComponentModel.DataAnnotations;

namespace Learnova.Domain.Entities
{
    public class Enrollment
    {

        [Key]
        public int EnrollmentId { get; set; }
        public string UserId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } // Active, Completed, Cancelled

        // Navigation
        public AppUser User { get; set; }
        public Course Course { get; set; }
    }
}
