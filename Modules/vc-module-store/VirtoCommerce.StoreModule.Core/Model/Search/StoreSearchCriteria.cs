using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.StoreModule.Core.Model.Search
{
    public class StoreSearchCriteria : SearchCriteriaBase
    {
        public string[] StoreIds { get; set; }

        public StoreState[] StoreStates { get; set; }
        public string[] FulfillmentCenterIds { get; set; }
    }
}
