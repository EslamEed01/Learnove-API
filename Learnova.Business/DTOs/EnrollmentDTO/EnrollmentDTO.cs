namespace Learnova.Business.DTOs.EnrollmentDTO
{
    public class EnrollmentDTO
    {
        public int EnrollmentId { get; set; }
        public string UserId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; }
    }
}