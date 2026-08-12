using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace HotelManagement.Infrastructure.Authorization;

public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission)
    {
        Policy = PermissionPolicyProvider.PolicyPrefix + permission;
    }
}

public sealed class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public const string PolicyPrefix = "Permission:";

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
    {
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(PolicyPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var permission = policyName[PolicyPrefix.Length..];
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                .AddRequirements(new PermissionRequirement(permission)).Build();
        }

        return await base.GetPolicyAsync(policyName);
    }
}