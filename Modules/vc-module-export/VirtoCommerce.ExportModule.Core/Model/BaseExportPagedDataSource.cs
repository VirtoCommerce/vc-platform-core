using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Core.Model
{
    public abstract class BaseExportPagedDataSource : IPagedDataSource
    {
        protected class FetchResult
        {
            public IEnumerable<IExportViewable> Results { get; set; }
            public int TotalCount { get; set; }

            public FetchResult(IEnumerable<IExportViewable> results, int totalCount)
            {
                Results = results;
                TotalCount = totalCount;
            }
        }

        //public int PageSize { get; set; } = 50;
        //public int CurrentPageNumber { get; private set; }

        public ExportDataQuery DataQuery { get; set; }
        private int _totalCount = -1;
        private SearchCriteriaBase _searchCriteria;

        protected BaseExportPagedDataSource()
        {
        }

        protected abstract FetchResult FetchData(SearchCriteriaBase searchCriteria);

        public virtual IEnumerable<IExportViewable> FetchNextPage()
        {
            EnsureSearchCriteriaInitialized();

            //_searchCriteria.Skip = _searchCriteria.Take * CurrentPageNumber;
            //_searchCriteria.Take = PageSize;

            var result = FetchData(_searchCriteria);
            _totalCount = result.TotalCount;
            _searchCriteria.Skip += _searchCriteria.Take;
            //CurrentPageNumber++;
            return result.Results;
        }

        public virtual int GetTotalCount()
        {
            if (_totalCount < 0)
            {
                var searchCriteria = DataQuery.ToSearchCriteria();

                searchCriteria.Skip = 0;
                searchCriteria.Take = 0;

                var result = FetchData(searchCriteria);
                _totalCount = result.TotalCount;
            }
            return _totalCount;
        }

        protected virtual void EnsureSearchCriteriaInitialized()
        {
            if (_searchCriteria == null)
            {
                _searchCriteria = DataQuery.ToSearchCriteria();
            }
        }
    }
}
