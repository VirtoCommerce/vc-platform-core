using System.Collections;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;

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

        public int PageSize { get; set; } = 50;
        public int CurrentPageNumber { get; private set; }
        public ExportDataQuery DataQuery { get; set; }
        private int _totalCount = -1;
        private SearchCriteriaBase _searchCriteria;

        protected abstract FetchResult FetchData(SearchCriteriaBase searchCriteria);

        public virtual IEnumerable FetchNextPage()
        {
            if (_searchCriteria == null)
            {
                _searchCriteria = DataQuery.ToSearchCriteria();
            }

            _searchCriteria.Skip = PageSize * CurrentPageNumber;
            _searchCriteria.Take = PageSize;

            var result = FetchData(_searchCriteria);
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

                var result = FetchData(_searchCriteria);
                _totalCount = result.TotalCount;
            }
            return _totalCount;
        }
    }
}
