using System.Collections.Generic;

namespace VirtoCommerce.Domain.Search
{
    public class SearchResponse
    {
        public long TotalCount { get; set; }
        public long DocumentsCount => Documents?.Count ?? 0;
        public IList<SearchDocument> Documents { get; set; } = new List<SearchDocument>();
        public IList<AggregationResponse> Aggregations { get; set; } = new List<AggregationResponse>();
    }
}
