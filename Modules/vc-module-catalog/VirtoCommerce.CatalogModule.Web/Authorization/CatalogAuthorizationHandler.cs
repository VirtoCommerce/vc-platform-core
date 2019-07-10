using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Security.Authorization;

namespace VirtoCommerce.CatalogModule.Web.Authorization
{
    public sealed class CatalogAuthorizationRequirement : PermissionAuthorizationRequirement
    {
        public CatalogAuthorizationRequirement(string permission)
            : base(permission)
        {
        }
    }

    public sealed class CatalogAuthorizationHandler : PermissionAuthorizationHandlerBase<CatalogAuthorizationRequirement>
    {
        private readonly MvcJsonOptions _jsonOptions;
        public CatalogAuthorizationHandler(IOptions<MvcJsonOptions> jsonOptions)
        {
            _jsonOptions = jsonOptions.Value;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CatalogAuthorizationRequirement requirement)
        {
            await base.HandleRequirementAsync(context, requirement);
            if (!context.HasSucceeded)
            {
                var userPermission = context.User.FindPermission(requirement.Permission, _jsonOptions.SerializerSettings);
                if (userPermission != null)
                {
                   
                }
            }
        }
    }
}
