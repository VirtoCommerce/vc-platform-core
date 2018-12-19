using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Security.Services;

namespace VirtoCommerce.Platform.Security.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionAuthorizationRequirement>
    {
        private readonly LimitedPermissionsHandler _limitedPermissionsHandler;

        public PermissionAuthorizationHandler(LimitedPermissionsHandler limitedPermissionsHandler)
        {
            _limitedPermissionsHandler = limitedPermissionsHandler;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionAuthorizationRequirement requirement)
        {
            var claims = context.User.Claims.ToArray();
            if (context.User.Identity.AuthenticationType == IdentityConstants.ApplicationScheme
                && _limitedPermissionsHandler.HasLimitedPermissionsClaim(claims))
            {
                if (await _limitedPermissionsHandler.UserHasAnyPermissionAsync(claims, requirement.Permission.Name))
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
                    return;
                }

                if (context.User.HasClaim(PlatformConstants.Security.Claims.PermissionClaimType, requirement.Permission.Name)
                    && context.User.HasClaim(PlatformConstants.Security.Claims.PermissionClaimType, PlatformConstants.Security.Permissions.SecurityCallApi))
                {
                    context.Succeed(requirement);
                }
            }

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
