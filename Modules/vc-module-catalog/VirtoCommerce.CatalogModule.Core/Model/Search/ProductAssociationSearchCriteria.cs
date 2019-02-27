using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model.Search
{
    public class ProductAssociationSearchCriteria : SearchCriteriaBase
    {
        public string Group { get; set; }
        public string[] Tags { get; set; }
    }
}
