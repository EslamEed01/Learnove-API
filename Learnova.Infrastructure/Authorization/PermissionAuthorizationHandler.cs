using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Learnova.Business.Abstraction;
using Learnova.Common.Entities;
using System.Security.Claims;

namespace Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public PermissionAuthorizationHandler(UserManager<AppUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine($"🔍 Permission check for: {requirement.Permission}, User ID: {userId}");
            
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("❌ No user ID found in claims");
                return;
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                Console.WriteLine("❌ User not found in database");
                return;
            }

            Console.WriteLine($"✅ User found: {user.Email}");

            var userRoles = await _userManager.GetRolesAsync(user);
            Console.WriteLine($"👤 User roles: {string.Join(", ", userRoles)}");
            
            var userClaims = await _userManager.GetClaimsAsync(user);
            Console.WriteLine($"📋 User has {userClaims.Count} direct claims");

            // Check if user has the required permission directly
            if (userClaims.Any(c => c.Type == Permissions.Type && c.Value == requirement.Permission))
            {
                Console.WriteLine("✅ Permission found in user claims");
                context.Succeed(requirement);
                return;
            }

            // Check if any of the user's roles have the required permission
            foreach (var roleName in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    Console.WriteLine($"🎭 Role '{roleName}' has {roleClaims.Count} permission claims");
                    
                    if (roleClaims.Any(c => c.Type == Permissions.Type && c.Value == requirement.Permission))
                    {
                        Console.WriteLine($"✅ Permission '{requirement.Permission}' found in role '{roleName}'");
                        context.Succeed(requirement);
                        return;
                    }
                    else
                    {
                        Console.WriteLine($"❌ Permission '{requirement.Permission}' NOT found in role '{roleName}'");
                        // Show what permissions this role actually has
                        var rolePermissions = roleClaims.Where(c => c.Type == Permissions.Type).Select(c => c.Value);
                        Console.WriteLine($"   Role permissions: {string.Join(", ", rolePermissions)}");
                    }
                }
                else
                {
                    Console.WriteLine($"❌ Role '{roleName}' not found in database");
                }
            }
            
            Console.WriteLine($"❌ Permission '{requirement.Permission}' not found anywhere");
        }
    }

    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}