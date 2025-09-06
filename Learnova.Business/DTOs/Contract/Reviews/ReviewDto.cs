namespace Learnova.Business.DTOs.Contract.Reviews
{
    public class ReviewDto
    {
        public int ReviewId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public int Rating { get; set; }
        public string? Title { get; set; }
        public string? Comment { get; set; }
        public DateTime ReviewDate { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public UserSummaryDto User { get; set; } = new();
    }

    public class UserSummaryDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
    }
}