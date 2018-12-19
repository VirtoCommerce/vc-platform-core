using System.Linq;
using Hangfire.Dashboard;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Security.Services;

namespace VirtoCommerce.Platform.Web.Hangfire
{
    public class HangfireAuthorizationHandler : IDashboardAuthorizationFilter
    {
        private readonly LimitedPermissionsHandler _limitedPermissionsHandler;

        public HangfireAuthorizationHandler(LimitedPermissionsHandler limitedPermissionsHandler)
        {
            _limitedPermissionsHandler = limitedPermissionsHandler;
        }

        public bool Authorize(DashboardContext context)
        {
            var httpcontext = context.GetHttpContext();
            var result = httpcontext != null && httpcontext.User.Identity.IsAuthenticated;
            if (result)
            {
                if (_limitedPermissionsHandler.HasLimitedPermissionsClaim(httpcontext.User.Claims.ToArray()))
                {
                    result = _limitedPermissionsHandler.UserHasAnyPermissionAsync(httpcontext.User.Claims.ToArray(), PlatformConstants.Security.Permissions.BackgroundJobsManage).GetAwaiter().GetResult();
                }
                else
                {
                    result = httpcontext.User.IsInRole(PlatformConstants.Security.Roles.Administrator) || httpcontext.User.HasClaim(PlatformConstants.Security.Claims.PermissionClaimType, PlatformConstants.Security.Permissions.BackgroundJobsManage);
                }
            }
            return result;
        }
    }
}
