namespace Learnova.Business.DTOs.Contract.Reviews
{
    public record ReviewResponse(
        int ReviewId,
        int CourseId,
        string CourseTitle,
        string UserId,
        string UserName,
        int Rating,
        string? Comment,
        DateTime ReviewDate,
        DateTime? UpdatedAt
    );
}