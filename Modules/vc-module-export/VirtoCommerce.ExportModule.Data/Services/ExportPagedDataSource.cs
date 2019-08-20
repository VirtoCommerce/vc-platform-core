using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Data.Services
{
    /// <summary>
    /// Base <see cref="IPagedDataSource"/> implementation for paginated homogenous data retrieving.
    /// <para/>
    /// Homogenous means only one search criteria is used by data source to query data.
    /// </summary>
    /// <typeparam name="TDataQuery">Specific <see cref="ExportDataQuery"/> type that is used to query this data source.</typeparam>
    /// <typeparam name="TSearchCriteria">Specific <see cref="SearchCriteriaBase"/> type that is used by this data source to query data.</typeparam>
    public abstract class ExportPagedDataSource<TDataQuery, TSearchCriteria> : IPagedDataSource
        where TDataQuery : ExportDataQuery
        where TSearchCriteria : SearchCriteriaBase
    {
        public IEnumerable<IExportable> Items { get; private set; }

        private readonly TDataQuery _dataQuery;
        protected int TotalCount = -1;

        protected ExportPagedDataSource(TDataQuery dataQuery)
        {
            _dataQuery = dataQuery;
        }

        public int CurrentPageNumber { get; protected set; } = 0;
        public int PageSize { get; set; } = 50;
        public virtual int GetTotalCount()
        {
            if (TotalCount < 0)
            {
                var searchCriteria = BuildSearchCriteria(_dataQuery);

                searchCriteria.Skip = 0;
                searchCriteria.Take = 0;

                TotalCount = FetchData(searchCriteria).TotalCount;
            }

            return TotalCount;
        }

        public bool Fetch()
        {
            var searchCriteria = BuildSearchCriteria(_dataQuery);
            var data = FetchData(searchCriteria);

            TotalCount = data.TotalCount;
            CurrentPageNumber++;

            Items = data.Results;

            return data.Results.Any();
        }


        protected virtual TSearchCriteria BuildSearchCriteria(TDataQuery exportDataQuery)
        {
            var result = AbstractTypeFactory<TSearchCriteria>.TryCreateInstance();

            result.ObjectIds = exportDataQuery.ObjectIds;
            result.Keyword = exportDataQuery.Keyword;
            result.Sort = exportDataQuery.Sort;

            // It is for proper pagination - client side for viewer (dataQuery.Skip/Take) should work together with iterating through pages when getting data for export
            result.Skip = exportDataQuery.Skip ?? CurrentPageNumber * PageSize;
            result.Take = exportDataQuery.Take ?? PageSize;

            return result;
        }

        /// <summary>
        /// Prepare exported data based on the <typeparamref name="TSearchCriteria"/> search criteria in this method.
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <returns></returns>
        protected abstract ExportableSearchResult FetchData(TSearchCriteria searchCriteria);
    }
}
