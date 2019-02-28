using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.SearchModule.Core.Services
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
        Task<SearchRequest> BuildRequestAsync(SearchCriteriaBase criteria);
    }
}
