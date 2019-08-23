using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VirtoCommerce.ExportModule.Data.Security
{
    /// <summary>
    /// Implements authorization for several policies, given to action by <see cref="AuthorizeAnyAttribute"/>.
    /// </summary>
    public class ExportAuthorizationFilter : IAuthorizationFilter, IAsyncAuthorizationFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthorizationService _authorizationService;

        public ExportAuthorizationFilter(IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor)
        {
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var policies = GetActionPolicies(context);
            var result = policies.Length == 0;
            foreach (var policy in policies)
            {
                result = result || _authorizationService.AuthorizeAsync(_httpContextAccessor.HttpContext.User, policy).GetAwaiter().GetResult().Succeeded;
                if (result) break;
            }
            if (!result) context.Result = new ForbidResult();
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var policies = GetActionPolicies(context);
            var result = policies.Length == 0;
            foreach (var policy in policies)
            {
                result = result || (await _authorizationService.AuthorizeAsync(_httpContextAccessor.HttpContext.User, policy)).Succeeded;
                if (result) break;
            }
            if (!result) context.Result = new ForbidResult();
        }

        /// <summary>
        /// Getting action policy names given by <see cref="AuthorizeAnyAttribute"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string[] GetActionPolicies(AuthorizationFilterContext context)
        {
            var result = new string[] { };
            var controllerActionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;
            var authorizeAnyAttribute = controllerActionDescriptor.MethodInfo.GetCustomAttributes(typeof(AuthorizeAnyAttribute), true).Cast<AuthorizeAnyAttribute>().FirstOrDefault();
            if (authorizeAnyAttribute != null)
            {
                result = authorizeAnyAttribute.Policies;
            }

            return result;
        }
    }
}
