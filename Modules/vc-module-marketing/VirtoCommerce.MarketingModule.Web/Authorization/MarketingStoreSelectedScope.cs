using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.MarketingModule.Web.Authorization
{
    /// <summary>
    /// Restricts access rights to a particular store
    /// </summary>
    public sealed class MarketingStoreSelectedScope : PermissionScope
    {
        public string StoreId => Scope; 
    }
}
