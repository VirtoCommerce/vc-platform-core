using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class PricingFullExportPagedDataSource : IPagedDataSource
    {
        private class ExportDataSourceState
        {
            public ExportDataSourceState()
            {
                Result = Array.Empty<IExportable>();
            }

            public int TotalCount;
            public int ReceivedCount;
            public ExportDataQuery DataQuery;
            public IExportable[] Result;
            public Func<ExportDataQuery, IPagedDataSource> DataSourceFactory;
        }

        public int TotalCount { get; set; }
        public int CurrentPageNumber { get; protected set; }
        public int PageSize { get; set; } = 50;

        private readonly Func<ExportDataQuery, PricelistExportPagedDataSource> _pricelistDataSourceFactory;
        private readonly Func<ExportDataQuery, PricelistAssignmentExportPagedDataSource> _assignmentsDataSourceFactory;
        private readonly Func<ExportDataQuery, PriceExportPagedDataSource> _pricesDataSourceFactory;

        private List<ExportDataSourceState> _exportDataSourceStates;
        public ExportDataQuery DataQuery
        {
            set
            {
                _dataQuery = value;
                CurrentPageNumber = 0;
                TotalCount = -1;

                _exportDataSourceStates = new List<ExportDataSourceState>()
                {
                    new ExportDataSourceState() {DataQuery = BuildExportDataQuery<PricelistExportDataQuery>(), DataSourceFactory = query =>  _pricelistDataSourceFactory(query)  },
                    new ExportDataSourceState() {DataQuery = BuildExportDataQuery<PricelistAssignmentExportDataQuery>(), DataSourceFactory =  query => _assignmentsDataSourceFactory(query)},
                    new ExportDataSourceState() {DataQuery = BuildExportDataQuery<PriceExportDataQuery>(), DataSourceFactory =  query =>_pricesDataSourceFactory(query)},
                };
                CalculateCounts();
            }
        }

        private ExportDataQuery _dataQuery;

        public PricingFullExportPagedDataSource(Func<ExportDataQuery, PricelistExportPagedDataSource> pricelistDataSourceFactory,
            Func<ExportDataQuery, PricelistAssignmentExportPagedDataSource> assignmentsDataSourceFactory,
            Func<ExportDataQuery, PriceExportPagedDataSource> pricesDataSourceFactory)
        {
            _pricelistDataSourceFactory = pricelistDataSourceFactory;
            _assignmentsDataSourceFactory = assignmentsDataSourceFactory;
            _pricesDataSourceFactory = pricesDataSourceFactory;
        }

        public int GetTotalCount()
        {
            CalculateCounts();
            return TotalCount;
        }

        private void EnsureHaveTotals()
        {
            if (TotalCount < 0)
            {
                CalculateCounts();
            }
        }

        public virtual IEnumerable<IExportable> FetchNextPage()
        {
            EnsureHaveTotals();
            var result = new List<IExportable>();

            int batchSize = _dataQuery.Take ?? PageSize;
            var taskList = new List<Task>();
            int skip = _dataQuery.Skip ?? CurrentPageNumber * PageSize;

            if (skip > 0)
            {
                foreach (var state in _exportDataSourceStates)
                {
                    if (state.TotalCount < skip)
                    {
                        state.ReceivedCount = state.TotalCount;
                        skip -= state.TotalCount;
                    }
                    else
                    {
                        state.ReceivedCount = skip;
                        skip = 0;
                    }
                }
            }

            foreach (var state in _exportDataSourceStates)
            {
                state.Result = Array.Empty<IExportable>();

                if (state.ReceivedCount < state.TotalCount && batchSize > 0)
                {
                    var portionCount = state.TotalCount - state.ReceivedCount > batchSize ? batchSize : state.TotalCount - state.ReceivedCount;
                    state.DataQuery.Take = portionCount;
                    state.DataQuery.Skip = state.ReceivedCount;
                    taskList.Add(Task.Factory.StartNew(() => { state.Result = state.DataSourceFactory(state.DataQuery).FetchNextPage().ToArray(); }));
                    batchSize = batchSize - portionCount;
                }
            }

            Task.WhenAll(taskList).GetAwaiter().GetResult();

            foreach (var state in _exportDataSourceStates)
            {
                result.AddRange(state.Result);
                state.ReceivedCount += state.Result.Length;
            }
            CurrentPageNumber++;

            return result;
        }


        private void CalculateCounts()
        {
            var taskList = new List<Task>();
            foreach (var state in _exportDataSourceStates)
            {
                state.DataQuery.Skip = 0;
                state.DataQuery.Take = 0;

                taskList.Add(Task.Factory.StartNew(() => { state.TotalCount = state.DataSourceFactory(state.DataQuery).GetTotalCount(); }));
            }

            Task.WhenAll(taskList).GetAwaiter().GetResult();

            TotalCount = _exportDataSourceStates.Sum(x => x.TotalCount);

        }
        private T BuildExportDataQuery<T>() where T : ExportDataQuery, new()
        {
            var result = AbstractTypeFactory<T>.TryCreateInstance();

            result.ObjectIds = _dataQuery.ObjectIds;
            result.Sort = _dataQuery.Sort;
            return result;
        }
    }
}
