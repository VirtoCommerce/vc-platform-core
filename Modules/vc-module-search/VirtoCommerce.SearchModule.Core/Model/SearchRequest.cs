using System.Collections.Generic;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.SearchModule.Core.Model
{
    /// <summary>
    /// A low level request which is passed to a search provider
    /// </summary>
    public class SearchRequest
    {
        /// <summary>
        /// Gets or sets keywords to search for in the indexed documents
        /// </summary>
        public string SearchKeywords { get; set; }

        /// <summary>
        /// Get or sets a list of indexed document fields where to search for keywords
        /// </summary>
        public IList<string> SearchFields { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use fuzzy search (true) or strict search (false)
        /// </summary>
        public bool IsFuzzySearch { get; set; }

        /// <summary>
        /// Gets or sets the maximum Levenshtein edit distance for a fuzzy search
        /// </summary>
        public int? Fuzziness { get; set; }

        /// <summary>
        /// Gets or sets a filter which narrows down the documents involved in the search operation
        /// </summary>
        public IFilter Filter { get; set; }

        /// <summary>
        /// Gets or sets a list of aggregation (facet) requests
        /// </summary>
        public IList<AggregationRequest> Aggregations { get; set; }

        /// <summary>
        /// Gets or sets a list of fields used to sort search results
        /// </summary>
        public IList<SortingField> Sorting { get; set; }

        /// <summary>
        /// Gets or sets the number of found documents to be skipped when building search results
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of documents to return in search results
        /// </summary>
        public int Take { get; set; } = 20;

        /// <summary>
        /// Gets or sets the search provider specific raw search query. If it has value, all other search criteria will be ignored.
        /// </summary>
        public string RawQuery { get; set; }
    }
}
