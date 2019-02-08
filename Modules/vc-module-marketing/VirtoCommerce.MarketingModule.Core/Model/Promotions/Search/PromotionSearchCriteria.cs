using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions.Search
{
    public class PromotionSearchCriteria : SearchCriteriaBase
    {
        public string Keyword { get; set; }
        public bool OnlyActive { get; set; }
        public string Store { get; set; }
        public string[] StoreIds { get; set; }
    }
}
