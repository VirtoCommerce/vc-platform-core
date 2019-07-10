using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.StoreModule.Web.Authorization
{
    /// <summary>
    /// Restricts access rights to a particular store
    /// </summary>
    public sealed class StoreSelectedScope : PermissionScope
    {
        public string StoreId => Scope; 
    }
}
