using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public class ProductAssociationSearchCriteria : SearchCriteriaBase
    {
        public string Group { get; set; }
        public string[] Tags { get; set; }
    }
}
