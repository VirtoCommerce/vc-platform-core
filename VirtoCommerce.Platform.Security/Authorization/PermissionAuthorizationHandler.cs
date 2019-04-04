using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.Platform.Core;

namespace VirtoCommerce.Platform.Security.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionAuthorizationRequirement requirement)
        {
            var limitedPermissionsClaim = context.User.FindFirstValue(PlatformConstants.Security.Claims.LimitedPermissionsClaimType);

            // LimitedPermissions claims that will be granted to the user by cookies when bearer token authentication is enabled.
            // This can help to authorize the user for direct(non - AJAX) GET requests to the VC platform API and / or to use some 3rd - party web applications for the VC platform(like Hangfire dashboard).
            //
            // If the user identity has claim named "limited_permissions", this attribute should authorize only permissions listed in that claim. Any permissions that are required by this attribute but
            // not listed in the claim should cause this method to return false. However, if permission limits of user identity are not defined ("limited_permissions" claim is missing),
            // then no limitations should be applied to the permissions.
            if (limitedPermissionsClaim != null)
            {
                var limitedPermissions = limitedPermissionsClaim.Split(PlatformConstants.Security.Claims.PermissionClaimTypeDelimiter, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];

                if (limitedPermissions.Contains(requirement.Permission.Name))
                {
                    context.Succeed(requirement);
                }
            }
            else
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
