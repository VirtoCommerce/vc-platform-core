using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.Platform.Security.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionAuthorizationRequirement requirement)
        {
            //TODO: Check cases with locked user
            if (context.User.IsInRole(PlatformConstants.Security.Roles.Administrator))
            {
                context.Succeed(requirement);
            }

            if (context.User.IsInRole(PlatformConstants.Security.Roles.Customer))
            {
                return Task.CompletedTask;
            }

            if (context.User.HasClaim(PlatformConstants.Security.Claims.PermissionClaimType, requirement.Permission.Name)
                && context.User.HasClaim(PlatformConstants.Security.Claims.PermissionClaimType, PlatformConstants.Security.Permissions.SecurityCallApi))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;

            //TODO: Check scoped permissions
            //if (result)
            //{
            //    var fqUserPermissions = user.Roles.SelectMany(x => x.Permissions).SelectMany(x => x.GetPermissionWithScopeCombinationNames()).Distinct();
            //    var fqCheckPermissions = permissionIds.Concat(permissionIds.LeftJoin(scopes, ":"));
            //    result = fqUserPermissions.Intersect(fqCheckPermissions, StringComparer.OrdinalIgnoreCase).Any();
            //}
        }


    }
}
