using System.Collections.Generic;

namespace VirtoCommerce.SearchModule.Core.Model
{
    public class TermAggregationRequest : AggregationRequest
    {
        public int? Size { get; set; }
        public IList<string> Values { get; set; }
    }
}
