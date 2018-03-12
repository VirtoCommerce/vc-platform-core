using VirtoCommerce.Domain.Search;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public class CategorySearchCriteria : CatalogSearchCriteriaBase
    {
        public override string ObjectType { get; set; } = KnownDocumentTypes.Category;
    }
}
