using System.ComponentModel.DataAnnotations;

namespace Learnova.Domain.Entities
{
    public class Category
    {

        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public ICollection<Course> Courses { get; set; }

    }
}
