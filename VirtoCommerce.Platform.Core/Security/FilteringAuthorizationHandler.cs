using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace VirtoCommerce.Platform.Core.Security
{
    public abstract class FilteringAuthorizationHandler<TRequirement, TResource> : PermissionScopeAuthorizationHandler<TRequirement, TResource>
        where TRequirement : PermissionScopeRequirement
    {
        protected FilteringAuthorizationHandler(UserManager<ApplicationUser> userManager) : base(userManager)
        {
        }

        protected override void HandlePermissions(AuthorizationHandlerContext context, TRequirement requirement, TResource resource, ApplicationUser user, List<PermissionScopeRequirement> permissionScopes)
        {
            // Modify search criteria to get only accessible entities
            if (permissionScopes.Any() && ApplyFilter(user, permissionScopes, resource))
            {
                context.Succeed(requirement);
            }
            // If we do not have any permissions on requirement.DesiredPermission at all - do not succeed requirement, search could not be done
        }

        /// <summary>
        /// Apply filtering function to resource.
        /// </summary>
        /// <param name="user">Current application user.</param>
        /// <param name="permissionScopes">Current user permission scopes.</param>
        /// <param name="resource">Resource to plly filter.</param>
        /// <returns>True if filter applied successfully, otherwise false.</returns>
        protected abstract bool ApplyFilter(ApplicationUser user, List<PermissionScopeRequirement> permissionScopes, TResource resource);
    }
}
