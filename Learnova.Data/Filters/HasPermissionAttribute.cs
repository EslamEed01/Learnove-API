using Microsoft.AspNetCore.Authorization;

namespace Learnova.Infrastructure.Filters
{
    public class HasPermissionAttribute(string permission) : AuthorizeAttribute(permission)
    {
    }
}
