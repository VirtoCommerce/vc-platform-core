using System.Collections.Generic;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Data.Services
{
    public abstract class SingleTypeExportPagedDataSource<TDataQuery, TSearchCriteria> : IPagedDataSource
        where TDataQuery : ExportDataQuery
        where TSearchCriteria : SearchCriteriaBase
    {
        protected TDataQuery _dataQuery;
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

        public virtual IEnumerable<IExportable> FetchNextPage()
        {
            var searchCriteria = BuildSearchCriteria(_dataQuery);
            var result = FetchData(searchCriteria as TSearchCriteria);

            _totalCount = result.TotalCount;
            CurrentPageNumber++;

            return result.Results;
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

        protected abstract void FillSearchCriteria(TDataQuery dataQuery, TSearchCriteria searchCriteria);

        protected abstract GenericSearchResult<IExportable> FetchData(TSearchCriteria searchCriteria);
    }
}
