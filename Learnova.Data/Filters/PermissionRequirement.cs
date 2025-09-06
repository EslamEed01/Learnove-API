using Microsoft.AspNetCore.Authorization;

namespace Learnova.Infrastructure.Filters
{
    public class PermissionRequirement(string permission) : IAuthorizationRequirement
    {
        public string Permission { get; } = permission;
    }
}
