using Microsoft.AspNetCore.Authorization;

namespace Learnova.Infrastructure.Filters
{
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(string permission)
            : base(policy: permission)
        {
        }
    }
}