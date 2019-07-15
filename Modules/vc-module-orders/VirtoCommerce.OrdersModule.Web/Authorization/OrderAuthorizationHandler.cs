using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VirtoCommerce.OrdersModule.Core;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Security.Authorization;

namespace VirtoCommerce.OrdersModule.Web.Authorization
{
    public sealed class OrderAuthorizationRequirement : PermissionAuthorizationRequirement
    {
        public OrderAuthorizationRequirement(string permission)
            : base(permission)
        {          
        }
    }

    public sealed class OrderAuthorizationHandler : PermissionAuthorizationHandlerBase<OrderAuthorizationRequirement>
    {
        private readonly MvcJsonOptions _jsonOptions;
        public OrderAuthorizationHandler(IOptions<MvcJsonOptions> jsonOptions)
        {
            _jsonOptions = jsonOptions.Value;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OrderAuthorizationRequirement requirement)
        {
            await base.HandleRequirementAsync(context, requirement);
            
            if (!context.HasSucceeded)
            {
                var userPermission = context.User.FindPermission(requirement.Permission, _jsonOptions.SerializerSettings);
                if (userPermission != null)
                {
                    var storeSelectedScopes = userPermission.AssignedScopes.OfType<OrderSelectedStoreScope>();
                    var onlyResponsibleScope = userPermission.AssignedScopes.OfType<OnlyOrderResponsibleScope>().FirstOrDefault();
                    var allowedStoreIds = storeSelectedScopes.Select(x => x.StoreId).Distinct().ToArray();

                    if (context.Resource is CustomerOrderSearchCriteria criteria)
                    {
                        criteria.StoreIds = allowedStoreIds;                       
                        if (onlyResponsibleScope != null)
                        {
                            criteria.EmployeeId = context.User.Identity.Name;
                        }                      
                        context.Succeed(requirement);                        
                    }
                    if (context.Resource is CustomerOrder order)
                    {                     
                        if (allowedStoreIds.Contains(order.StoreId) || (onlyResponsibleScope != null && order.EmployeeId.EqualsInvariant(context.User.Identity.Name)))
                        {
                            context.Succeed(requirement);
                        }
                    }
                }
            }

            //Apply ReadPrices authorization rules for all checks
            if (!context.User.HasGlobalPermission(ModuleConstants.Security.Permissions.ReadPrices))
            {
                if (context.Resource is CustomerOrderSearchCriteria criteria)
                {
                    if (string.IsNullOrEmpty(criteria.ResponseGroup))
                    {
                        criteria.ResponseGroup = CustomerOrderResponseGroup.Full.ToString();
                    }
                    criteria.ResponseGroup = EnumUtility.SafeRemoveFlagFromEnumString(criteria.ResponseGroup, CustomerOrderResponseGroup.WithPrices);
                    //Do not allow pass empty response group into services because that can leads to use default response group CustomerOrderResponseGroup.Full
                    if (string.IsNullOrEmpty(criteria.ResponseGroup))
                    {
                        criteria.ResponseGroup = CustomerOrderResponseGroup.Default.ToString();
                    }
                    context.Succeed(requirement);
                }
                if (context.Resource is CustomerOrder order)
                {
                    order.ReduceDetails((CustomerOrderResponseGroup.Full & ~CustomerOrderResponseGroup.WithPrices).ToString());
                }           
            }
        }
    }
}
