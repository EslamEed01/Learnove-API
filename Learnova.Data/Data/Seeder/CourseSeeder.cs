using Learnova.Domain.Entities;

namespace Learnova.Infrastructure.Data.Seeder
{
    public class CourseSeeder
    {
        public static List<Course> Get()
        {
            return new List<Course>
            {
                // Web Development Courses
                new Course
                {
                    CourseId = 1,
                    CategoryId = 1,
                    Title = "Complete React.js Bootcamp",
                    Description = "Master React.js from basics to advanced concepts. Build real-world projects and learn modern React patterns, hooks, context API, and state management with Redux.",
                    ImageUrl = "https://example.com/images/react-bootcamp.jpg",
                    Price = 89.99m,
                    CreatedBy = UserSeeder.Instructor1Id,
                    CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                    LessonsCount = 45
                },

                new Course
                {
                    CourseId = 2,
                    CategoryId = 1,
                    Title = "Node.js and Express.js Masterclass",
                    Description = "Build scalable backend applications with Node.js and Express.js. Learn REST APIs, authentication, database integration, and deployment strategies.",
                    ImageUrl = "https://example.com/images/nodejs-masterclass.jpg",
                    Price = 79.99m,
                    CreatedBy = UserSeeder.Instructor1Id,
                    CreatedAt = new DateTime(2024, 1, 20, 0, 0, 0, DateTimeKind.Utc),
                    LessonsCount = 38
                },

                new Course
                {
                    CourseId = 3,
                    CategoryId = 1, // Web Development
                    Title = "Full-Stack Web Development with MERN",
                    Description = "Complete full-stack development course using MongoDB, Express.js, React.js, and Node.js. Build and deploy modern web applications.",
                    ImageUrl = "https://example.com/images/mern-stack.jpg",
                    Price = 129.99m,
                    CreatedBy = UserSeeder.Instructor1Id,
                    CreatedAt = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                    LessonsCount = 62
                },

                new Course
                {
                    CourseId = 4,
                    CategoryId = 2,
                    Title = "UI/UX Design Fundamentals",
                    Description = "Learn the principles of user interface and user experience design. Master design thinking, wireframing, prototyping, and user research methodologies.",
                    ImageUrl = "https://example.com/images/uiux-fundamentals.jpg",
                    Price = 69.99m,
                    CreatedBy = UserSeeder.Instructor2Id,
                    CreatedAt = new DateTime(2024, 1, 25, 0, 0, 0, DateTimeKind.Utc),
                    LessonsCount = 32
                },

                new Course
                {
                    CourseId = 5,
                    CategoryId = 2,
                    Title = "Advanced Figma for Designers",
                    Description = "Master Figma's advanced features including auto-layout, components, design systems, prototyping, and collaboration tools for professional design workflows.",
                    ImageUrl = "https://example.com/images/figma-advanced.jpg",
                    Price = 59.99m,
                    CreatedBy = UserSeeder.Instructor2Id,
                    CreatedAt = new DateTime(2024, 2, 5, 0, 0, 0, DateTimeKind.Utc),
                    LessonsCount = 28
                },

                new Course
                {
                    CourseId = 6,
                    CategoryId = 2,
                    Title = "Mobile App Design with Flutter",
                    Description = "Design beautiful and functional mobile applications. Learn mobile design patterns, Material Design, iOS guidelines, and responsive design principles.",
                    ImageUrl = "https://example.com/images/mobile-design.jpg",
                    Price = 74.99m,
                    CreatedBy = UserSeeder.Instructor2Id,
                    CreatedAt = new DateTime(2024, 2, 10, 0, 0, 0, DateTimeKind.Utc),
                    LessonsCount = 35
                },

                new Course
                {
                    CourseId = 7,
                    CategoryId = 3,
                    Title = "C# Programming for Beginners",
                    Description = "Start your programming journey with C#. Learn object-oriented programming, .NET framework, and build console and desktop applications.",
                    ImageUrl = "https://example.com/images/csharp-beginners.jpg",
                    Price = 49.99m,
                    CreatedBy = UserSeeder.Instructor1Id,
                    CreatedAt = new DateTime(2024, 1, 30, 0, 0, 0, DateTimeKind.Utc),
                    LessonsCount = 40
                },

                new Course
                {
                    CourseId = 8,
                    CategoryId = 3,
                    Title = "Python Data Science Bootcamp",
                    Description = "Master Python for data science with pandas, NumPy, matplotlib, and scikit-learn. Learn data analysis, visualization, and machine learning fundamentals.",
                    ImageUrl = "https://example.com/images/python-datascience.jpg",
                    Price = 99.99m,
                    CreatedBy = UserSeeder.Instructor1Id,
                    CreatedAt = new DateTime(2024, 2, 8, 0, 0, 0, DateTimeKind.Utc),
                    LessonsCount = 55
                },

                new Course
                {
                    CourseId = 9,
                    CategoryId = 4,
                    Title = "Digital Marketing Mastery",
                    Description = "Complete guide to digital marketing including SEO, social media marketing, email marketing, content strategy, and analytics.",
                    ImageUrl = "https://example.com/images/digital-marketing.jpg",
                    Price = 64.99m,
                    CreatedBy = UserSeeder.Instructor2Id,
                    CreatedAt = new DateTime(2024, 2, 12, 0, 0, 0, DateTimeKind.Utc),
                    LessonsCount = 42
                },

                new Course
                {
                    CourseId = 10,
                    CategoryId = 4,
                    Title = "Entrepreneurship and Startup Fundamentals",
                    Description = "Learn how to start and grow a successful business. Cover business planning, funding, marketing strategies, and scaling operations.",
                    ImageUrl = "https://example.com/images/entrepreneurship.jpg",
                    Price = 79.99m,
                    CreatedBy = UserSeeder.Instructor2Id,
                    CreatedAt = new DateTime(2024, 2, 15, 0, 0, 0, DateTimeKind.Utc),
                    LessonsCount = 36
                },

                new Course
                {
                    CourseId = 11,
                    CategoryId = 1,
                    Title = "HTML & CSS Basics (Free)",
                    Description = "Free introductory course to web development. Learn HTML structure, CSS styling, responsive design basics, and create your first website.",
                    ImageUrl = "https://example.com/images/html-css-basics.jpg",
                    Price = 0.00m,
                    CreatedBy = UserSeeder.Instructor1Id,
                    CreatedAt = new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc),
                    LessonsCount = 20
                },

                new Course
                {
                    CourseId = 12,
                    CategoryId = 2,
                    Title = "Design Principles (Free)",
                    Description = "Free course covering fundamental design principles including color theory, typography, composition, and visual hierarchy for beginners.",
                    ImageUrl = "https://example.com/images/design-principles.jpg",
                    Price = 0.00m,
                    CreatedBy = UserSeeder.Instructor2Id,
                    CreatedAt = new DateTime(2024, 1, 12, 0, 0, 0, DateTimeKind.Utc),
                    LessonsCount = 15
                }
            };
        }
    }
}