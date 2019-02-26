namespace VirtoCommerce.CatalogModule.Core.Model.Search
{
    public class CategorySearchCriteria : CatalogSearchCriteriaBase
    {
        public override string ObjectType { get; set; } = KnownDocumentTypes.Category;
    }
}
