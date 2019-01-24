using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.Security
{

    public abstract class PermissionScopeAuthorizationHandler<TRequirement, TResource> : AuthorizationHandler<TRequirement, TResource>
        where TRequirement : PermissionScopeRequirement
    {
        protected readonly UserManager<ApplicationUser> _userManager;
        protected PermissionScopeAuthorizationHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement, TResource resource)
        {
            var userName = context.User.Identity.Name;
            var user = await _userManager.FindByNameAsync(userName) ?? throw new InvalidOperationException($"Cannot find order with user \"{userName}\".");

            if (user.IsAdministrator || (await _userManager.IsInRoleAsync(user, PlatformConstants.Security.Roles.Administrator)))
            {
                context.Succeed(requirement);
                return;
            }

            // Get all roles permissions for this requirement
            var permissions = user.Roles.SelectMany(x => x.Permissions).Where(x => x.Name.EqualsInvariant(requirement.DesiredPermission)).ToList();

            // If we have this permission without scope - authorization succeeded.
            if (permissions.Any(x => (x.AssignedScopes == null || x.AssignedScopes.Count == 0)))
            {
                context.Succeed(requirement);
                return;
            }

            // If we have desired permission only with scopes - get all scopes
            var permissionScopes = permissions.SelectMany(x => x.AssignedScopes).ToList();

            HandlePermissions(context, requirement, resource, user, permissionScopes);
        }

        protected abstract void HandlePermissions(AuthorizationHandlerContext context, TRequirement requirement, TResource resource, ApplicationUser user, List<PermissionScopeRequirement> permissionScopes);
    }
}
