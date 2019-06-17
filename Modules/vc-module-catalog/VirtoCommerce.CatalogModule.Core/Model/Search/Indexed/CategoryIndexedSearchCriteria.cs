using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Core.Model.Search
{
    public class CategoryIndexedSearchCriteria : CatalogIndexedSearchCriteria
    {
        public override string ObjectType { get; set; } = KnownDocumentTypes.Category;
    }
}
