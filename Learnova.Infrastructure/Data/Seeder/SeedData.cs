using Learnova.Business.Abstraction;
using Learnova.Business.Abstraction.Consts;
using Learnova.Common.Entities;
using Learnova.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Learnova.Infrastructure.Data.Seeder
{
    public static class SeedData
    {
        public static async Task InitializeAsync(LearnoveDbContext context, IServiceProvider serviceProvider)
        {
            try
            {
                // Check if users already exist
                var hasUsers = await context.Users.AnyAsync();

                if (!hasUsers)
                {
                    Console.WriteLine("🌱 Seeding database...");
                    
                    var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
                    var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

                    await CreateRolesAsync(roleManager);
                    await CreateAdminUserAsync(userManager, roleManager);
                    
                    Console.WriteLine("✅ Database seeding completed");
                }
                else
                {
                    Console.WriteLine("📊 Database already seeded");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error seeding database: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private static async Task CreateRolesAsync(RoleManager<ApplicationRole> roleManager)
        {
            var roles = new[]
            {
                DefaultRoles.admin,
                DefaultRoles.instructor,
                DefaultRoles.student
            };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new ApplicationRole { Name = roleName };
                    var result = await roleManager.CreateAsync(role);
                    
                    if (!result.Succeeded)
                    {
                        Console.WriteLine($"❌ Failed to create role {roleName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                        continue;
                    }
                    
                    // Add permissions to roles
                    if (roleName == DefaultRoles.admin)
                    {
                        var adminPermissions = Permissions.GetAdminPermissions();
                        foreach (var permission in adminPermissions)
                        {
                            await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim(Permissions.Type, permission));
                        }
                        Console.WriteLine($"✅ Created Admin role with {adminPermissions.Count} permissions");
                    }
                    else if (roleName == DefaultRoles.instructor)
                    {
                        var teacherPermissions = Permissions.GetTeacherPermissions();
                        foreach (var permission in teacherPermissions)
                        {
                            await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim(Permissions.Type, permission));
                        }
                        Console.WriteLine($"✅ Created Instructor role with {teacherPermissions.Count} permissions");
                    }
                    else if (roleName == DefaultRoles.student)
                    {
                        var studentPermissions = Permissions.GetStudentPermissions();
                        foreach (var permission in studentPermissions)
                        {
                            await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim(Permissions.Type, permission));
                        }
                        Console.WriteLine($"✅ Created Student role with {studentPermissions.Count} permissions");
                    }
                }
                else
                {
                    Console.WriteLine($"📝 Role {roleName} already exists");
                }
            }
        }

        private static async Task CreateAdminUserAsync(UserManager<AppUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            const string adminEmail = "admin@learnova.com";
            const string adminPassword = "Admin@123";

            var existingUser = await userManager.FindByEmailAsync(adminEmail);
            if (existingUser == null)
            {
                var adminUser = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true,
                    RoleType = "admin"
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, DefaultRoles.admin);
                    Console.WriteLine($"✅ Created admin user: {adminEmail} / {adminPassword}");
                }
                else
                {
                    Console.WriteLine($"❌ Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                Console.WriteLine($"📝 Admin user already exists: {adminEmail}");
                
                // Make sure admin user has the admin role
                var isInRole = await userManager.IsInRoleAsync(existingUser, DefaultRoles.admin);
                if (!isInRole)
                {
                    await userManager.AddToRoleAsync(existingUser, DefaultRoles.admin);
                    Console.WriteLine($"✅ Added Admin role to existing user: {adminEmail}");
                }
            }
        }
    }
}