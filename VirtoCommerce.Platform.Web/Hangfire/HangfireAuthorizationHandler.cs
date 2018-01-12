using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.Platform.Web.Hangfire
{
    public class HangfireAuthorizationHandler : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpcontext = context.GetHttpContext();
            var result = httpcontext != null && httpcontext.User.Identity.IsAuthenticated;
            if(result)
            {
                result = httpcontext.User.IsInRole(SecurityConstants.Roles.Administrator) || httpcontext.User.HasClaim(SecurityConstants.Claims.PermissionClaimType, SecurityConstants.Permissions.BackgroundJobsManage);
            }
            return result;
        }
    }
}
