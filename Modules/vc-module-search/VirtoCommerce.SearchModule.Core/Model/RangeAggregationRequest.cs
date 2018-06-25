using System.Collections.Generic;

namespace VirtoCommerce.SearchModule.Core.Model
{
    public class RangeAggregationRequest : AggregationRequest
    {
        public IList<RangeAggregationRequestValue> Values { get; set; }
    }
}
