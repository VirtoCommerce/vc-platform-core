using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.OrdersModule.Web.Authorization
{
    /// <summary>
    /// Restricts access rights to orders that belong to a particular store
    /// </summary>
    public sealed class OrderSelectedStoreScope : PermissionScope
    {
        public string StoreId => Scope;
    }
}
