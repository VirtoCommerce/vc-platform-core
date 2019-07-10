using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.CatalogModule.Web.Authorization
{
    /// <summary>
    /// Restricts access rights to a particular category
    /// </summary>
    public sealed class SelectedCategoryScope : PermissionScope
    {
        public string CategoryId => Scope;
    }
}
