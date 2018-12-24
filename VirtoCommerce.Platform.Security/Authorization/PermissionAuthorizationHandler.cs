using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Security.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionAuthorizationRequirement>
    {
        public const string LimitedPermissionsClaimName = "LimitedPermissions";

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionAuthorizationRequirement requirement)
        {
            var claims = context.User.Claims.ToArray();
            var limitedPermissionsClaim = claims.FirstOrDefault(c => c.Type.EqualsInvariant(LimitedPermissionsClaimName));

            if (context.User.Identity.AuthenticationType == IdentityConstants.ApplicationScheme
                && limitedPermissionsClaim != null)
            {
                var limitedPermissions = limitedPermissionsClaim.Value?.Split(';', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];

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
