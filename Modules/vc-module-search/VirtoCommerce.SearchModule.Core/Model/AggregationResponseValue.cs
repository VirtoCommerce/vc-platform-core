using System.Diagnostics;

namespace VirtoCommerce.SearchModule.Core.Model
{
    [DebuggerDisplay("{Id}: {Count}")]
    public class AggregationResponseValue
    {
        public string Id { get; set; }
        public long Count { get; set; }
    }
}
