using VirtoCommerce.Platform.Security.Authorization;

namespace VirtoCommerce.PricingModule.Data.Authorization
{
    public class PricingAuthorizationRequirement : PermissionAuthorizationRequirement
    {
        public PricingAuthorizationRequirement(string permission)
            : base(permission)
        {
        }
    }
    public class PricingAuthorizationHandler : PermissionAuthorizationHandlerBase<PricingAuthorizationRequirement>
    {

    }
}
