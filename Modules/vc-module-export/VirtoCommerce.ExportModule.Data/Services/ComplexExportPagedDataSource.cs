using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Data.Services
{
    /// <summary>
    /// Base data source for getting heterogeneous data from different data sources, e.g. several object types from different services.
    /// </summary>
    /// <typeparam name="TDataQuery">ExportDataQuery for that data source.</typeparam>
    public abstract class ComplexExportPagedDataSource<TDataQuery> : IPagedDataSource
        where TDataQuery : ExportDataQuery
    {
        /// <summary>
        /// Data source state object allowing to fetch data from the source.
        /// </summary>
        protected class ExportDataSourceState
        {
            public int TotalCount;
            public SearchCriteriaBase SearchCriteria;
            public IEnumerable<IExportable> Result = Array.Empty<IExportable>();
            public Func<ExportDataSourceState, Task> FetchFunc;
        }

        public int TotalCount { get; set; }
        public int CurrentPageNumber { get; protected set; }
        public int PageSize { get; set; } = 50;

        public IEnumerable<IExportable> Items { get; protected set; }

        protected readonly List<ExportDataSourceState> _exportDataSourceStates = new List<ExportDataSourceState>();

        protected TDataQuery DataQuery;

        protected ComplexExportPagedDataSource(TDataQuery dataQuery)
        {
            SetDataQuery(dataQuery);
        }

        protected void SetDataQuery(TDataQuery dataQuery)
        {
            DataQuery = dataQuery;
            CurrentPageNumber = 0;
            TotalCount = -1;

            InitDataSourceStates();
        }

        protected abstract void InitDataSourceStates();

        public int GetTotalCount()
        {
            EnsureHaveTotals();

            return TotalCount;
        }

        protected void EnsureHaveTotals()
        {
            if (TotalCount < 0)
            {
                CalculateCounts();
            }
        }

        public bool Fetch()
        {
            EnsureHaveTotals();

            var take = DataQuery.Take ?? PageSize;
            var skip = DataQuery.Skip ?? CurrentPageNumber * PageSize;
            var taskList = new List<Task>();
            var result = new List<IExportable>();

            foreach (var state in _exportDataSourceStates)
            {
                state.Result = Array.Empty<IExportable>();

                if (take <= 0)
                {
                    break;
                }
                else if (state.TotalCount <= skip)
                {
                    skip -= state.TotalCount;
                }
                else
                {
                    var portionCount = (state.TotalCount - skip > take) ? take : state.TotalCount - skip;
                    state.SearchCriteria.Take = portionCount;
                    state.SearchCriteria.Skip = skip;
                    taskList.Add(state.FetchFunc(state));
                    take -= portionCount;
                    skip = 0;
                }
            }

            Task.WhenAll(taskList).GetAwaiter().GetResult();

            Items = _exportDataSourceStates.SelectMany(x => x.Result).ToList();
            CurrentPageNumber++;

            return Items.Any();
        }

        protected void CalculateCounts()
        {
            var taskList = new List<Task>();

            foreach (var state in _exportDataSourceStates)
            {
                state.SearchCriteria.Skip = 0;
                state.SearchCriteria.Take = 0;
                taskList.Add(state.FetchFunc(state));
            }

            Task.WhenAll(taskList).GetAwaiter().GetResult();
            TotalCount = _exportDataSourceStates.Sum(x => x.TotalCount);
        }
    }
}
