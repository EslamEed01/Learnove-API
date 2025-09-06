namespace Learnova.Infrastructure.Data
{
    public static class Permissions
    {
        public static string Type { get; } = "permissions";

        // Course Management
        public const string GetCourses = "courses:read";
        public const string AddCourses = "courses:add";
        public const string UpdateCourses = "courses:update";
        public const string DeleteCourses = "courses:delete";
        public const string EnrollCourses = "courses:enroll";
        public const string PublishCourses = "courses:publish";

        // Lesson Management
        public const string GetLessons = "lessons:read";
        public const string AddLessons = "lessons:add";
        public const string UpdateLessons = "lessons:update";
        public const string DeleteLessons = "lessons:delete";


        // Student Management
        public const string GetStudents = "students:read";
        public const string AddStudents = "students:add";
        public const string UpdateStudents = "students:update";
        public const string DeleteStudents = "students:delete";

        // Instructor Management
        public const string GetInstructors = "instructors:read";
        public const string AddInstructors = "instructors:add";
        public const string UpdateInstructors = "instructors:update";
        public const string DeleteInstructors = "instructors:delete";

        // User Management
        public const string GetUsers = "users:read";
        public const string AddUsers = "users:add";
        public const string UpdateUsers = "users:update";

        // Role Management
        public const string GetRoles = "roles:read";
        public const string AddRoles = "roles:add";
        public const string UpdateRoles = "roles:update";

        public static IList<string?> GetAllPermissions() =>
            typeof(Permissions).GetFields().Select(x => x.GetValue(x) as string).ToList();
    }
}
