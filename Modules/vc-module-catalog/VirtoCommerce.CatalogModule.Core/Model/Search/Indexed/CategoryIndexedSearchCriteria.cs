using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public class CategoryIndexedSearchCriteria : IndexedSearchCriteriaBase
    {
        public override string ObjectType { get; set; } = KnownDocumentTypes.Category;
    }
}
