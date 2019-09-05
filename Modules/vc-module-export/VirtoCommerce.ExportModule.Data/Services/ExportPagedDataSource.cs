using System;
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
        public IEnumerable<IExportable> Items { get; protected set; }

        public ExportDataQuery DataQuery => _dataQuery;

        private readonly TDataQuery _dataQuery;
        protected int TotalCount = -1;

        protected ExportPagedDataSource(TDataQuery dataQuery)
        {
            _dataQuery = dataQuery ?? throw new ArgumentNullException(nameof(dataQuery));
        }

        public int CurrentPageNumber { get; protected set; }
        public int PageSize { get; set; } = 50;
        public int? Skip { get => _dataQuery.Skip; set => _dataQuery.Skip = value; }
        public int? Take { get => _dataQuery.Take; set => _dataQuery.Take = value; }

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
            var hasData = true;
            var searchCriteria = BuildSearchCriteria(_dataQuery);
            var hasObjectIds = !searchCriteria.ObjectIds.IsNullOrEmpty();

            if (hasObjectIds)
            {
                searchCriteria.ObjectIds = searchCriteria.ObjectIds.Skip(searchCriteria.Skip).Take(searchCriteria.Take).ToArray();
            }

            if (hasObjectIds && searchCriteria.ObjectIds.IsNullOrEmpty())
            {
                hasData = false;
            }

            if (hasData)
            {
                var data = FetchData(searchCriteria);

                CurrentPageNumber++;
                Items = data.Results;

                hasData = data.Results.Any();
            }
            else
            {
                Items = Array.Empty<IExportable>();
            }

            return hasData;
        }


        protected virtual TSearchCriteria BuildSearchCriteria(TDataQuery exportDataQuery)
        {
            var result = AbstractTypeFactory<TSearchCriteria>.TryCreateInstance();

            result.ObjectIds = exportDataQuery.ObjectIds;
            result.Keyword = exportDataQuery.Keyword;
            result.Sort = exportDataQuery.Sort;

            // It is for proper pagination - client side for viewer (dataQuery.Skip/Take) should work together with iterating through pages when getting data for export
            result.Skip = Skip ?? CurrentPageNumber * PageSize;
            result.Take = Take ?? PageSize;

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
