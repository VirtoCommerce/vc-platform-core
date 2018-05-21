using System.Collections.Generic;

namespace VirtoCommerce.Domain.Search
{
    public class TermAggregationRequest : AggregationRequest
    {
        public int? Size { get; set; }
        public IList<string> Values { get; set; }
    }
}
