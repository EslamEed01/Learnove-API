using Learnova.Infrastructure.Data;
using Learnova.Infrastructure.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly ILogger<PermissionAuthorizationHandler> _logger;

        public PermissionAuthorizationHandler(ILogger<PermissionAuthorizationHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            _logger.LogInformation("Checking permission: {Permission}", requirement.Permission);
            _logger.LogInformation("User authenticated: {IsAuthenticated}", context.User.Identity?.IsAuthenticated);
            _logger.LogInformation("User claims count: {ClaimsCount}", context.User.Claims.Count());

            foreach (var claim in context.User.Claims)
            {
                _logger.LogInformation("Claim: Type={Type}, Value={Value}", claim.Type, claim.Value);
            }

            if (context.User.Identity is not { IsAuthenticated: true })
            {
                _logger.LogWarning("User not authenticated");
                return;
            }

            var hasPermission = context.User.Claims.Any(x => x.Value == requirement.Permission && x.Type == Permissions.Type);
            _logger.LogInformation("Has permission {Permission}: {HasPermission}", requirement.Permission, hasPermission);

            if (!hasPermission)
            {
                _logger.LogWarning("User does not have required permission: {Permission}", requirement.Permission);
                return;
            }

            context.Succeed(requirement);
        }
    }
}