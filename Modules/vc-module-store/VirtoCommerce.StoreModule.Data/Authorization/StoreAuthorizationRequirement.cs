using VirtoCommerce.Platform.Security.Authorization;

namespace VirtoCommerce.StoreModule.Data.Authorization
{
    public sealed class StoreAuthorizationRequirement : PermissionAuthorizationRequirement
    {
        public StoreAuthorizationRequirement(string permission)
            : base(permission)
        {
        }
    }
}
