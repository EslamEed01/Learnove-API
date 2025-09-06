using Learnova.Domain.Entities;

namespace Learnova.Infrastructure.Data.Seeder
{
    public static class RoleSeeder
    {
        public static readonly string AdminRoleId = "admin-role-id-12345";
        public static readonly string InstructorRoleId = "instructor-role-id-12345";
        public static readonly string StudentRoleId = "student-role-id-12345";

        public static List<ApplicationRole> GetRoles()
        {
            return new List<ApplicationRole>
            {
                new ApplicationRole
                {
                    Id = AdminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    IsDefault = true,
                    IsDeleted = false,
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
                new ApplicationRole
                {
                    Id = InstructorRoleId,
                    Name = "Instructor",
                    NormalizedName = "INSTRUCTOR",
                    IsDefault = true,
                    IsDeleted = false,
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
                new ApplicationRole
                {
                    Id = StudentRoleId,
                    Name = "Student",
                    NormalizedName = "STUDENT",
                    IsDefault = true,
                    IsDeleted = false,
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                }
            };
        }

        public static List<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>> GetRoleClaims()
        {
            var allPermissions = Permissions.GetAllPermissions().Where(p => !string.IsNullOrEmpty(p)).ToList();
            var roleClaims = new List<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>>();

            // Give Admin all permissions
            for (int i = 0; i < allPermissions.Count; i++)
            {
                roleClaims.Add(new Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>
                {
                    Id = i + 1,
                    RoleId = AdminRoleId,
                    ClaimType = Permissions.Type,
                    ClaimValue = allPermissions[i]
                });
            }

            return roleClaims;
        }

        public static List<Microsoft.AspNetCore.Identity.IdentityUserRole<string>> GetUserRoles()
        {
            return new List<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>
            {
                new Microsoft.AspNetCore.Identity.IdentityUserRole<string>
                {
                    UserId = UserSeeder.AdminId,
                    RoleId = AdminRoleId
                }
            };
        }
    }
}