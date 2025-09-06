using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Learnova.Infrastructure.Authorization
{
    public class PermissionAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly AuthorizationOptions _options;

        public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _options = options.Value;
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => Task.FromResult(_options.DefaultPolicy);

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => Task.FromResult(_options.FallbackPolicy);

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith("Permissions."))
            {
                var policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new PermissionRequirement(policyName))
                    .Build();

                return Task.FromResult<AuthorizationPolicy?>(policy);
            }

            return Task.FromResult<AuthorizationPolicy?>(null);
        }
    }
}