using System.Collections.Generic;
using VirtoCommerce.Domain.Catalog.Model.Search;

namespace VirtoCommerce.CatalogModule.Data.Search.BrowseFilters
{
    public interface IBrowseFilterService
    {
        /// <summary>
        /// Returns aggregations that match the specified search criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IList<IBrowseFilter> GetBrowseFilters(ProductSearchCriteria criteria);

        /// <summary>
        /// Returns all aggregations configured for a store
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        IList<IBrowseFilter> GetStoreAggregations(string storeId);

        /// <summary>
        /// Binds aggregations to a store
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="filters"></param>
        void SaveStoreAggregations(string storeId, IList<IBrowseFilter> filters);
    }
}
