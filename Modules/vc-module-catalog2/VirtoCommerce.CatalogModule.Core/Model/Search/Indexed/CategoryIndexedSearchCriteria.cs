using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Core2.Model.Search.Indexed
{
    public class CategoryIndexedSearchCriteria : IndexedSearchCriteriaBase
    {
        public override string ObjectType { get; set; } = KnownDocumentTypes.Category;
    }
}
