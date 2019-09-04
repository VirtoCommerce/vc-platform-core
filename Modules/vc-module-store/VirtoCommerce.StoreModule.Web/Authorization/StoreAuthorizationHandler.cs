using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Security.Authorization;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Model.Search;
using VirtoCommerce.StoreModule.Data.Authorization;

namespace VirtoCommerce.StoreModule.Web.Authorization
{
    public sealed class StoreAuthorizationHandler : PermissionAuthorizationHandlerBase<StoreAuthorizationRequirement>
    {
        private readonly MvcJsonOptions _jsonOptions;
        public StoreAuthorizationHandler(IOptions<MvcJsonOptions> jsonOptions)
        {
            _jsonOptions = jsonOptions.Value;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, StoreAuthorizationRequirement requirement)
        {
            await base.HandleRequirementAsync(context, requirement);
            if (!context.HasSucceeded)
            {
                var userPermission = context.User.FindPermission(requirement.Permission, _jsonOptions.SerializerSettings);
                if (userPermission != null)
                {
                    var storeSelectedScopes = userPermission.AssignedScopes.OfType<StoreSelectedScope>();
                    var allowedStoreIds = storeSelectedScopes.Select(x => x.StoreId).Distinct().ToArray();
                    if (context.Resource is StoreSearchCriteria criteria)
                    {
                        criteria.StoreIds = allowedStoreIds;
                        context.Succeed(requirement);
                    }
                    if (context.Resource is Store store && allowedStoreIds.Contains(store.Id))
                    {
                        context.Succeed(requirement);
                    }
                    if (context.Resource is string[] storeIds && storeIds.Intersect(allowedStoreIds).Count() == storeIds.Count())
                    {
                        context.Succeed(requirement);
                    }
                }
            }
        }
    }
}
