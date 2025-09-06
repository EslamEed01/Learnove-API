using Learnova.Infrastructure.Data.Context;

namespace Learnova.Infrastructure.Data.Seeder
{
    public static class SeedData
    {
        public static async Task InitializeAsync(LearnoveDbContext context)
        {
            if (!context.Users.Any())
            {
                context.Users.AddRange(UserSeeder.Get());
            }

            if (!context.Roles.Any())
            {
                context.Roles.AddRange(RoleSeeder.GetRoles());
                context.RoleClaims.AddRange(RoleSeeder.GetRoleClaims());
                context.UserRoles.AddRange(RoleSeeder.GetUserRoles());
            }

            if (!context.Courses.Any())
            {
                context.Courses.AddRange(CourseSeeder.Get());
            }

            await context.SaveChangesAsync();


        }
    }
}
