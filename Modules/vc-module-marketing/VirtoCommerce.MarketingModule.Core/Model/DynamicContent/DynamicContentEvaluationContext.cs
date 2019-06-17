using System;
using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model
{
    public class DynamicContentEvaluationContext : EvaluationContextBase
    {
        public DynamicContentEvaluationContext()
        {
        }

        public DynamicContentEvaluationContext(string storeId, string placeName, DateTime toDate, string[] tags)
        {
            StoreId = storeId;
            PlaceName = placeName;
            ToDate = toDate;
            Tags = tags;
        }

        public string StoreId { get; set; }

        public string PlaceName { get; set; }

        public string[] Tags { get; set; }

        public DateTime ToDate { get; set; }



    }
}
