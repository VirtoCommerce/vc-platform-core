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
    public abstract class SingleTypeExportPagedDataSource<TDataQuery, TSearchCriteria> : IPagedDataSource
        where TDataQuery : ExportDataQuery
        where TSearchCriteria : SearchCriteriaBase
    {
        protected TDataQuery _dataQuery;
        public IEnumerable<IExportable> Items { get; private set; }

        protected int _totalCount = -1;
        public int CurrentPageNumber { get; protected set; }
        public int PageSize { get; set; } = 50;

        public TDataQuery DataQuery
        {
            set
            {
                _dataQuery = value;

                //Reset datasource state when DataQuery is changed
                CurrentPageNumber = 0;
                _totalCount = -1;
            }
        }

        public virtual bool Fetch()
        {
            var searchCriteria = BuildSearchCriteria(_dataQuery);
            var data = FetchData(searchCriteria);

            _totalCount = data.TotalCount;
            CurrentPageNumber++;

            Items = data.Results;

            return data.Results.Any();
        }

        public virtual int GetTotalCount()
        {
            if (_totalCount < 0)
            {
                var searchCriteria = BuildSearchCriteria(_dataQuery);

                searchCriteria.Skip = 0;
                searchCriteria.Take = 0;

                _totalCount = FetchData(searchCriteria).TotalCount;
            }

            return _totalCount;
        }

        protected virtual TSearchCriteria BuildSearchCriteria(TDataQuery exportDataQuery)
        {
            var result = AbstractTypeFactory<TSearchCriteria>.TryCreateInstance();

            FillSearchCriteria(exportDataQuery, result);

            result.ObjectIds = exportDataQuery.ObjectIds;
            result.Keyword = exportDataQuery.Keyword;
            result.Sort = exportDataQuery.Sort;

            // It is for proper pagination - client side for viewer (dataQuery.Skip/Take) should work together with iterating through pages when getting data for export
            result.Skip = exportDataQuery.Skip ?? CurrentPageNumber * PageSize;
            result.Take = exportDataQuery.Take ?? PageSize;

            return result;
        }

        /// <summary>
        /// Fill <paramref name="searchCriteria"/> with <paramref name="dataQuery"/> data specific fields here.
        /// Some common <see cref="SearchCriteriaBase"/> fields are already filled.
        /// </summary>
        /// <param name="dataQuery"></param>
        /// <param name="searchCriteria"></param>
        protected abstract void FillSearchCriteria(TDataQuery dataQuery, TSearchCriteria searchCriteria);

        /// <summary>
        /// Prepare exported data based on the <typeparamref name="TSearchCriteria"/> search criteria in this method.
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <returns></returns>
        protected abstract ExportableSearchResult FetchData(TSearchCriteria searchCriteria);
    }
}
