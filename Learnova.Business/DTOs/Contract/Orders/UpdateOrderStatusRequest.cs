using System.ComponentModel.DataAnnotations;

namespace Learnova.Business.DTOs.Contract.Orders
{
    public record UpdateOrderStatusRequest
    {
        [Required]
        [RegularExpression("^(Pending|Completed|Cancelled|Failed)$",
            ErrorMessage = "Payment status must be one of: Pending, Completed, Cancelled, Failed")]
        public string PaymentStatus { get; init; } = string.Empty;
    }
}