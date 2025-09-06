namespace Authorization
{
    public static class Permissions
    {
        public const string Type = "permission";

        // Role permissions
        public const string GetRoles = "permissions.roles.read";
        public const string AddRoles = "permissions.roles.create";
        public const string UpdateRoles = "permissions.roles.update";
        public const string DeleteRoles = "permissions.roles.delete";

        // User permissions
        public const string GetUsers = "permissions.users.read";
        public const string AddUsers = "permissions.users.create";
        public const string UpdateUsers = "permissions.users.update";
        public const string DeleteUsers = "permissions.users.delete";

        // Course permissions
        public const string GetCourses = "permissions.courses.read";
        public const string AddCourses = "permissions.courses.create";
        public const string UpdateCourses = "permissions.courses.update";
        public const string DeleteCourses = "permissions.courses.delete";

        public static IReadOnlyList<string> GetAllPermissions() =>
            new[]
            {
                GetRoles, AddRoles, UpdateRoles, DeleteRoles,
                GetUsers, AddUsers, UpdateUsers, DeleteUsers,
                GetCourses, AddCourses, UpdateCourses, DeleteCourses
            };
    }
}