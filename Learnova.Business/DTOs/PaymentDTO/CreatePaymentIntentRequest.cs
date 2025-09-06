using System.ComponentModel.DataAnnotations;

namespace Learnova.Business.DTOs.PaymentDTO
{
    public class CreatePaymentIntentRequest
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string Currency { get; set; } = "usd";

        public int? OrderId { get; set; }
    }
}
