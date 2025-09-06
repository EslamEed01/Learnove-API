using Learnova.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Learnova.Infrastructure.Data.Seeder
{
    public class UserSeeder
    {


        public static readonly string AdminId = "admin-id-12345-67890-abcdef";
        public static readonly string Instructor1Id = "instructor1-id-12345-67890-abcdef";
        public static readonly string Instructor2Id = "instructor2-id-12345-67890-abcdef";
        public static readonly string Student1Id = "student1-id-12345-67890-abcdef";
        public static readonly string Student2Id = "student2-id-12345-67890-abcdef";

        public static List<AppUser> Get()
        {
            var passwordHasher = new PasswordHasher<AppUser>();


            return new List<AppUser>
        {


                  new AppUser
                {
                    Id = AdminId,
                    UserName = "admin@learnova.com",
                    NormalizedUserName = "ADMIN@LEARNOVA.COM",
                    Email = "admin@learnova.com",
                    NormalizedEmail = "ADMIN@LEARNOVA.COM",
                    EmailConfirmed = true,
                    PasswordHash = passwordHasher.HashPassword(null, "Admin123!"),
                    SecurityStamp = "admin-security-stamp-12345",
                    ConcurrencyStamp = "admin-concurrency-stamp-12345",
                    FirstName = "John",
                    LastName = "Admin",
                    DateOfBirth = new DateTime(1985, 5, 15),
                    ProfilePictureUrl = "https://example.com/images/admin.jpg",
                    Bio = "System Administrator with expertise in e-learning platforms and user management.",
                    RoleType = "admin",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    PhoneNumber = "+1-555-0101",
                    PhoneNumberConfirmed = true,
                    LockoutEnabled = false,
                    TwoFactorEnabled = false,
                    AccessFailedCount = 0
                },

                // Instructor 1
                new AppUser
                {
                    Id = Instructor1Id,
                    UserName = "sarah.instructor@learnova.com",
                    NormalizedUserName = "SARAH.INSTRUCTOR@LEARNOVA.COM",
                    Email = "sarah.instructor@learnova.com",
                    NormalizedEmail = "SARAH.INSTRUCTOR@LEARNOVA.COM",
                    EmailConfirmed = true,
                    PasswordHash = passwordHasher.HashPassword(null, "Instructor123!"),
                    SecurityStamp = "instructor1-security-stamp-12345",
                    ConcurrencyStamp = "instructor1-concurrency-stamp-12345",
                    FirstName = "Sarah",
                    LastName = "Johnson",
                    DateOfBirth = new DateTime(1990, 8, 22),
                    ProfilePictureUrl = "https://example.com/images/sarah.jpg",
                    Bio = "Experienced software developer and instructor with 8+ years in the industry. Specializes in web development and modern JavaScript frameworks.",
                    RoleType = "instructor",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    PhoneNumber = "+1-555-0102",
                    PhoneNumberConfirmed = true,
                    LockoutEnabled = false,
                    TwoFactorEnabled = false,
                    AccessFailedCount = 0
                },

                // Instructor 2
                new AppUser
                {
                    Id = Instructor2Id,
                    UserName = "jane.instructor@learnova.com",
                    NormalizedUserName = "JANE.INSTRUCTOR@LEARNOVA.COM",
                    Email = "jane.instructor@learnova.com",
                    NormalizedEmail = "JANE.INSTRUCTOR@LEARNOVA.COM",
                    EmailConfirmed = true,
                    PasswordHash = passwordHasher.HashPassword(null, "Instructor456!"),
                    SecurityStamp = "instructor2-security-stamp-12345",
                    ConcurrencyStamp = "instructor2-concurrency-stamp-12345",
                    FirstName = "Jane",
                    LastName = "Smith",
                    DateOfBirth = new DateTime(1988, 3, 18),
                    ProfilePictureUrl = "https://example.com/images/jane.jpg",
                    Bio = "UI/UX Designer and instructor specializing in design thinking, user experience, and creative problem-solving methodologies.",
                    RoleType = "instructor",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    PhoneNumber = "+1-555-0104",
                    PhoneNumberConfirmed = true,
                    LockoutEnabled = false,
                    TwoFactorEnabled = true,
                    AccessFailedCount = 0
                },

                // Student 1
                new AppUser
                {
                    Id = Student1Id,
                    UserName = "mike.student@learnova.com",
                    NormalizedUserName = "MIKE.STUDENT@LEARNOVA.COM",
                    Email = "mike.student@learnova.com",
                    NormalizedEmail = "MIKE.STUDENT@LEARNOVA.COM",
                    EmailConfirmed = true,
                    PasswordHash = passwordHasher.HashPassword(null, "Student123!"),
                    SecurityStamp = "student1-security-stamp-12345",
                    ConcurrencyStamp = "student1-concurrency-stamp-12345",
                    FirstName = "Mike",
                    LastName = "Davis",
                    DateOfBirth = new DateTime(1995, 12, 10),
                    ProfilePictureUrl = "https://example.com/images/mike.jpg",
                    Bio = "Passionate learner pursuing web development skills. Currently focusing on full-stack development and modern frameworks.",
                    RoleType = "student",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    PhoneNumber = "+1-555-0103",
                    PhoneNumberConfirmed = false,
                    LockoutEnabled = true,
                    TwoFactorEnabled = false,
                    AccessFailedCount = 0
                },

                // Student 2
                new AppUser
                {
                    Id = Student2Id,
                    UserName = "alex.student@learnova.com",
                    NormalizedUserName = "ALEX.STUDENT@LEARNOVA.COM",
                    Email = "alex.student@learnova.com",
                    NormalizedEmail = "ALEX.STUDENT@LEARNOVA.COM",
                    EmailConfirmed = false,
                    PasswordHash = passwordHasher.HashPassword(null, "Student789!"),
                    SecurityStamp = "student2-security-stamp-12345",
                    ConcurrencyStamp = "student2-concurrency-stamp-12345",
                    FirstName = "Alex",
                    LastName = "Wilson",
                    DateOfBirth = new DateTime(1997, 7, 5),
                    ProfilePictureUrl = null,
                    Bio = "Computer science student eager to learn programming and data science. Interested in machine learning and AI technologies.",
                    RoleType = "student",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    PhoneNumber = "+1-555-0105",
                    PhoneNumberConfirmed = false,
                    LockoutEnabled = true,
                    TwoFactorEnabled = false,
                    AccessFailedCount = 0







        }






            };
        }
    }
}
