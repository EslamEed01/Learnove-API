using System.ComponentModel.DataAnnotations;

namespace Learnova.Domain.Entities
{
    public class Order
    {

        [Key]
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public int PaymentId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }

        // Navigation
        public AppUser User { get; set; }
        public Payment Payment { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
