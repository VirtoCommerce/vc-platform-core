using System.Collections.Generic;

namespace VirtoCommerce.Domain.Search
{
    public class RangeAggregationRequest : AggregationRequest
    {
        public IList<RangeAggregationRequestValue> Values { get; set; }
    }
}
