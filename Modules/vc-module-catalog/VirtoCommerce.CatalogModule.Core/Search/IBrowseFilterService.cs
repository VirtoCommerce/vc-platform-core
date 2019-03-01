using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model.Search;

namespace VirtoCommerce.CatalogModule.Core.Search
{
    public interface IBrowseFilterService
    {
        /// <summary>
        /// Returns aggregations that match the specified search criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        Task<IList<IBrowseFilter>> GetBrowseFiltersAsync(ProductSearchCriteria criteria);

        /// <summary>
        /// Returns all aggregations configured for a store
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        Task<IList<IBrowseFilter>> GetStoreAggregations(string storeId);

        /// <summary>
        /// Binds aggregations to a store
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="filters"></param>
        Task SaveStoreAggregations(string storeId, IList<IBrowseFilter> filters);
    }
}
