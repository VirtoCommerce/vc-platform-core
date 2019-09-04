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

                var currentSkip = Math.Min(state.TotalCount, skip);
                var currentTake = Math.Min(take, Math.Max(0, state.TotalCount - skip));

                if (currentSkip <= 0 && currentTake <= 0)
                {
                    break;
                }
                else if (currentSkip < state.TotalCount && currentTake > 0)
                {
                    state.SearchCriteria.Take = currentTake;
                    state.SearchCriteria.Skip = currentSkip;
                    taskList.Add(state.FetchFunc(state));
                }

                skip -= currentSkip;
                take -= currentTake;
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
