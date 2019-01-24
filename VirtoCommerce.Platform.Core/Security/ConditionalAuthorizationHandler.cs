using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace VirtoCommerce.Platform.Core.Security
{
    public abstract class ConditionalAuthorizationHandler<TRequirement, TResource> : PermissionScopeAuthorizationHandler<TRequirement, TResource>
        where TRequirement : PermissionScopeRequirement
    {
        protected ConditionalAuthorizationHandler(UserManager<ApplicationUser> userManager) : base(userManager)
        {
        }

        protected override void HandlePermissions(AuthorizationHandlerContext context, TRequirement requirement, TResource resource, ApplicationUser user, List<PermissionScopeRequirement> permissionScopes)
        {
            // And check if we have condition valid in any scope.
            if (permissionScopes.Any(x => EvaluateCondition(user, x, resource)))
            {
                context.Succeed(requirement);
            }
        }

        /// <summary>
        /// Evaluates custom condition for authorization requirement.
        /// </summary>
        /// <param name="user">Current user.</param>
        /// <param name="scope">Permission scope.</param>
        /// <param name="resource">Resource to check.</param>
        /// <returns>True if condition is true, otherwise false.</returns>
        protected abstract bool EvaluateCondition(ApplicationUser user, PermissionScopeRequirement scope, TResource resource);
    }
}
