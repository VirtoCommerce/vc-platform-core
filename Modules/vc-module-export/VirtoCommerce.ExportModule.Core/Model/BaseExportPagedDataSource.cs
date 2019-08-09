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
            public IEnumerable<ICloneable> Results { get; set; }
            public int TotalCount { get; set; }

            public FetchResult(IEnumerable<ICloneable> results, int totalCount)
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

        protected BaseExportPagedDataSource()
        {
        }

        protected abstract FetchResult FetchData(SearchCriteriaBase searchCriteria);

        protected abstract ViewableEntity ToViewableEntity(object obj);

        public virtual IEnumerable<ICloneable> FetchNextPage()
        {
            EnsureSearchCriteriaInitialized();

            _searchCriteria.Skip = PageSize * CurrentPageNumber;
            _searchCriteria.Take = PageSize;

            var result = FetchData(_searchCriteria);
            _totalCount = result.TotalCount;
            CurrentPageNumber++;
            return result.Results;
        }

        public virtual ViewableSearchResult GetData()
        {
            EnsureSearchCriteriaInitialized();

            var queryResult = FetchData(_searchCriteria);
            var result = new ViewableSearchResult()
            {
                TotalCount = queryResult.TotalCount,
            };

            result.Results = ToViewableEntities(queryResult.Results).ToList();

            return result;
        }

        public virtual int GetTotalCount()
        {
            if (_totalCount < 0)
            {
                EnsureSearchCriteriaInitialized();

                _searchCriteria.Skip = 0;
                _searchCriteria.Take = 0;

                var result = FetchData(_searchCriteria);
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

        protected virtual IEnumerable<ViewableEntity> ToViewableEntities(IEnumerable<ICloneable> objects)
        {
            return objects.Select(x => ToViewableEntity(x));
        }
    }
}
