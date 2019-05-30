using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.TaxModule.Core.Model.Search
{
    public class TaxProviderSearchCriteria : SearchCriteriaBase
    {
        public string StoreId { get; set; }
        //Search only within tax providers that have changes and persisted
        public bool WithoutTransient { get; set; } = false;
    }
}
