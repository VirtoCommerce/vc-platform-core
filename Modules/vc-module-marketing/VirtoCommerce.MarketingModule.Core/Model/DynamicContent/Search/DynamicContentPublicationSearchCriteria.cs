using System;

namespace VirtoCommerce.MarketingModule.Core.Model
{
    public class DynamicContentPublicationSearchCriteria : DynamicContentSearchCriteriaBase
    {
        public bool OnlyActive { get; set; }
        public string Store { get; set; }
        public string PlaceName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual DynamicContentPublicationSearchCriteria FromEvalContext(DynamicContentEvaluationContext context)
        {
            Store = context.StoreId;
            PlaceName = context.PlaceName;
            EndDate = context.ToDate;
            return this;
        }
    }
}
