using System.ComponentModel.DataAnnotations;

namespace Learnova.Business.DTOs.CateDTO
{
    public class CategoryDTO
    {

        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
