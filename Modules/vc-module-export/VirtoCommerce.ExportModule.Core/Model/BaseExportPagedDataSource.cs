using System.Collections;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Core.Model
{
    public abstract class BaseExportPagedDataSource : IPagedDataSource
    {
        protected class FetchResult
        {
            public IEnumerable Results { get; set; }
            public int TotalCount { get; set; }

            public FetchResult(IEnumerable results, int totalCount)
            {
                Results = results;
                TotalCount = totalCount;
            }
        }

        public int PageSize { get; set; }
        public int CurrentPageNumber { get; set; }
        public ExportDataQuery DataQuery { get; set; }
        private int _totalCount = -1;
        private SearchCriteriaBase _searchCriteria;

        protected abstract FetchResult FetchUsingService(SearchCriteriaBase searchCriteria);

        public virtual IEnumerable FetchNextPage()
        {
            if (_searchCriteria == null)
            {
                _searchCriteria = DataQuery.ToSearchCriteria();
            }

            _searchCriteria.Skip = PageSize * CurrentPageNumber;
            _searchCriteria.Take = CurrentPageNumber;

            var result = FetchUsingService(_searchCriteria);
            _totalCount = result.TotalCount;
            CurrentPageNumber++;
            return result.Results;
        }

        public virtual int GetTotalCount()
        {
            if (_totalCount < 0)
            {
                if (_searchCriteria == null)
                {
                    _searchCriteria = DataQuery.ToSearchCriteria();
                }

                _searchCriteria.Skip = 0;
                _searchCriteria.Take = 0;

                var result = FetchUsingService(_searchCriteria);
                _totalCount = result.TotalCount;
            }
            return _totalCount;
        }
    }
}
