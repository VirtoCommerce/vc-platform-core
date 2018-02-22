using VirtoCommerce.Domain.Search;

namespace VirtoCommerce.Domain.Catalog.Model.Search
{
    public class CategorySearchCriteria : CatalogSearchCriteriaBase
    {
        public override string ObjectType { get; set; } = KnownDocumentTypes.Category;
    }
}
