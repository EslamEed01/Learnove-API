using System.ComponentModel.DataAnnotations;

namespace Learnova.Business.DTOs.Contract.Orders
{
    public record CreateOrderRequest
    {
        [Required(ErrorMessage = "Course IDs are required")]
        [MinLength(1, ErrorMessage = "At least one course must be specified")]
        public List<int> CourseIds { get; init; } = new();

        [Required(ErrorMessage = "Payment method is required")]
        [StringLength(50, ErrorMessage = "Payment method cannot exceed 50 characters")]
        public string PaymentMethod { get; init; } = string.Empty;
    };
}