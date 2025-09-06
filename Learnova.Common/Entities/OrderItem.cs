using System.ComponentModel.DataAnnotations;

namespace Learnova.Domain.Entities
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int CourseId { get; set; }
        public decimal Price { get; set; }

        // Navigation properties
        public Order Order { get; set; }
        public Course Course { get; set; }
    }
}