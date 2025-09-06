namespace Learnova.Business.DTOs.Contract.Orders
{
    public record OrderResponse(
        int OrderId,
        string UserId,
        DateTime OrderDate,
        decimal TotalPrice,
        string PaymentMethod,
        string PaymentStatus,
        List<OrderCourseResponse> Courses
    );
}