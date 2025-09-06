using Learnova.Business.Abstraction;
using Microsoft.AspNetCore.Http;

namespace Learnova.Business.Errors
{
    public static class OrderErrors
    {
        public static readonly Error OrderNotFound = new("Order.NotFound", "Order not found", StatusCodes.Status404NotFound);
        public static readonly Error InvalidCourses = new("Order.InvalidCourses", "One or more courses are invalid or not found", StatusCodes.Status400BadRequest);
        public static readonly Error EmptyOrder = new("Order.EmptyOrder", "Order must contain at least one course", StatusCodes.Status400BadRequest);
        public static readonly Error OrderAlreadyPaid = new("Order.AlreadyPaid", "Order has already been paid", StatusCodes.Status400BadRequest);
        public static readonly Error OrderAlreadyCancelled = new("Order.AlreadyCancelled", "Order has already been cancelled", StatusCodes.Status400BadRequest);
        public static readonly Error UserNotFound = new("Order.UserNotFound", "User not found", StatusCodes.Status404NotFound);
        public static readonly Error PaymentCreationFailed = new("Order.PaymentCreationFailed", "Failed to create payment record", StatusCodes.Status500InternalServerError);
        public static readonly Error AlreadyEnrolled = new("Order.AlreadyEnrolled", "User is already enrolled in one or more courses", StatusCodes.Status400BadRequest);
        public static readonly Error DuplicateCourses = new("Order.DuplicateCourses", "Duplicate courses found in order", StatusCodes.Status400BadRequest);
    }
}