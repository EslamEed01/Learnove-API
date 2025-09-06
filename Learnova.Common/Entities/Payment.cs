using System.ComponentModel.DataAnnotations;

namespace Learnova.Domain.Entities
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public string? PaymentIntentId { get; set; }

        // Navigation
        public Order Order { get; set; }
    }
}
