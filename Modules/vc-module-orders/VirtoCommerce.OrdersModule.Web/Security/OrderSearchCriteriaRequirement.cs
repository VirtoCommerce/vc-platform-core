using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.OrdersModule.Web.Security
{
    public class OrderSearchCriteriaRequirement : PermissionScopeRequirement
    {
        public const string PolicyName = "OrderSearchCriteria";
    }

    public class OrderSearchCriteriaAutorizationHandler : FilteringAuthorizationHandler<OrderSearchCriteriaRequirement, CustomerOrderSearchCriteria>
    {
        public OrderSearchCriteriaAutorizationHandler(UserManager<ApplicationUser> userManager) : base(userManager)
        {
        }

        protected override bool ApplyFilter(ApplicationUser user, List<PermissionScopeRequirement> permissionScopes, CustomerOrderSearchCriteria resource)
        {
            var result = true;
            // Stores
            var accessibleStores = permissionScopes.OfType<OrderStoreRequirement>()
                .Where(x => !string.IsNullOrEmpty(x.Scope))
                .Select(x => x.Scope)
                .ToArray();
            resource.StoreIds = (resource.StoreIds == null)
                ? accessibleStores
                : resource.StoreIds.Intersect(accessibleStores, StringComparer.InvariantCultureIgnoreCase).ToArray();

            // Employee id
            var responsibleScope = permissionScopes.OfType<OrderResponsibleRequirement>().FirstOrDefault();
            if (responsibleScope != null)
            {
                if (resource.EmployeeId == null || resource.EmployeeId.EqualsInvariant(user.Id))
                {
                    resource.EmployeeId = user.Id;
                }
                else // Trying to search orders for the Employee we do not have rights for should fail authorization
                {
                    result = false;
                }
            }
            return result;
        }
    }
}
