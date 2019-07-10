using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.CatalogModule.Web.Authorization
{
    /// <summary>
    /// Restricts access rights to a particular catalog
    /// </summary>
    public sealed class SelectedCatalogScope : PermissionScope
    {
        public string CatalogId => Scope;
    }
}
