using System.Collections.Generic;
using System.Diagnostics;

namespace VirtoCommerce.SearchModule.Core.Model
{
    public class AggregationResponse
    {
        public string Id { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public IList<AggregationResponseValue> Values { get; set; }
    }
}
