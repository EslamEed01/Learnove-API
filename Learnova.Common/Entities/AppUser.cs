using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace Learnova.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDisabled { get; set; }
        public string RoleType { get; set; } // student, instructor, admin

        public List<RefreshToken> RefreshTokens { get; set; } = [];

        //Navation prop

        [JsonIgnore]
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        [JsonIgnore]
        public ICollection<Course> CreatedCourses { get; set; }

        [JsonIgnore]
        public ICollection<Enrollment> Enrollments { get; set; }

        [JsonIgnore]
        public ICollection<Order> Orders { get; set; }

        [JsonIgnore]
        public ICollection<pdfContents> pdf_Contents { get; set; }
    }
}
