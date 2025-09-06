namespace Learnova.Business.DTOs.EnrollmentDTO
{
    public class MemberDTO
    {
        public int EnrollmentId { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; }
    }
}
