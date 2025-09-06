namespace Learnova.Business.DTOs.Contract.Orders
{
    public record OrderCourseResponse(
        int CourseId,
        string Title,
        decimal Price
    );
}