using System.Diagnostics;

namespace VirtoCommerce.Domain.Search
{
    [DebuggerDisplay("{Id}: {Count}")]
    public class AggregationResponseValue
    {
        public string Id { get; set; }
        public long Count { get; set; }
    }
}
