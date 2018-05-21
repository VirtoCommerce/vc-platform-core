using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Domain.Search
{
    /// <summary>
    /// Search services use this interface to convert search criteria to a search request which is then passed to a search provider.
    /// </summary>
    public interface ISearchRequestBuilder
    {
        /// <summary>
        /// Gets the type of documents for which this builder can build search requests (Product, Category, etc.)
        /// </summary>
        string DocumentType { get; }

        /// <summary>
        /// Builds a search request based on given search criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        SearchRequest BuildRequest(SearchCriteriaBase criteria);
    }
}
