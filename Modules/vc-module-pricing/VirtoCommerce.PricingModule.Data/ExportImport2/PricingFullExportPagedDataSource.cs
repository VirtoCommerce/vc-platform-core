using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class PricingFullExportPagedDataSource : IPagedDataSource
    {
        private readonly Func<ExportDataQuery, PricelistExportPagedDataSource> _pricelistDataSourceFactory;
        private readonly Func<ExportDataQuery, PricelistAssignmentExportPagedDataSource> _assignmentsDataSourceFactory;
        private readonly Func<ExportDataQuery, PriceExportPagedDataSource> _pricesDataSourceFactory;

        private PricelistExportPagedDataSource _pricelistDataSource;
        private PricelistAssignmentExportPagedDataSource _assignmentsDataSource;
        private PriceExportPagedDataSource _pricesDataSource;

        private PricelistExportDataQuery _pricelistExportDataQuery;
        private PricelistAssignmentExportDataQuery _pricelistAssignmentExportDataQuery;
        private PriceExportDataQuery _priceExportDataQuery;

        private T BuildExportDataQuery<T>() where T : ExportDataQuery, new()
        {
            var newExportDataQuery = new T();
            newExportDataQuery.Skip = DataQuery.Skip;
            newExportDataQuery.Take = DataQuery.Take;
            newExportDataQuery.ObjectIds = DataQuery.ObjectIds;
            newExportDataQuery.Sort = DataQuery.Sort;
            newExportDataQuery.IncludedColumns = DataQuery.IncludedColumns.Where(x => x.ExportName.StartsWith("Pricelist.")).ToArray();
            return newExportDataQuery;
        }
        public ExportDataQuery DataQuery
        {
            get => _dataQuery;
            set
            {
                _dataQuery = value;

                //Reset datasource state when DataQuery is changed
                CurrentPageNumber = 0;
                TotalCount = -1;
                _pricelistExportDataQuery = BuildExportDataQuery<PricelistExportDataQuery>();
                _priceExportDataQuery = BuildExportDataQuery<PriceExportDataQuery>();
                _pricelistAssignmentExportDataQuery = BuildExportDataQuery<PricelistAssignmentExportDataQuery>();

                _pricelistDataSource = _pricelistDataSourceFactory(_pricelistExportDataQuery);
                _assignmentsDataSource = _assignmentsDataSourceFactory(_pricelistAssignmentExportDataQuery);
                _pricesDataSource = _pricesDataSourceFactory(_priceExportDataQuery);
                CalculateCounts();

            }
        }


        public int PricesTotalCount { get; set; }
        public int AssignmentsTotalCount { get; set; }
        public int PricelistsTotalCount { get; set; }


        private ExportDataQuery _dataQuery;


        public int TotalCount { get; set; }

        public PricingFullExportPagedDataSource(Func<ExportDataQuery, PricelistExportPagedDataSource> pricelistDataSourceFactory,
            Func<ExportDataQuery, PricelistAssignmentExportPagedDataSource> assignmentsDataSourceFactory,
            Func<ExportDataQuery, PriceExportPagedDataSource> pricesDataSourceFactory)
        {
            _pricelistDataSourceFactory = pricelistDataSourceFactory;
            _assignmentsDataSourceFactory = assignmentsDataSourceFactory;
            _pricesDataSourceFactory = pricesDataSourceFactory;
        }

        public int CurrentPageNumber { get; protected set; }
        public int PageSize { get; set; } = 50;

        private void CalculateCounts()
        {
            int totalCount = 0;
            var dataQuery1 = _pricelistExportDataQuery.Clone() as PricelistExportDataQuery;
            dataQuery1.Skip = 0;
            dataQuery1.Take = 0;

            var dataQuery2 = _priceExportDataQuery.Clone() as PriceExportDataQuery;
            dataQuery2.Skip = 0;
            dataQuery2.Take = 0;

            var dataQuery3 = _pricelistAssignmentExportDataQuery.Clone() as PricelistAssignmentExportDataQuery;
            dataQuery3.Skip = 0;
            dataQuery3.Take = 0;
            var taskList = new List<Task>();
            taskList.Add(Task.Factory.StartNew(() => { PricelistsTotalCount = _pricelistDataSourceFactory(dataQuery1).GetTotalCount(); }));
            taskList.Add(Task.Factory.StartNew(() => { AssignmentsTotalCount = _assignmentsDataSourceFactory(dataQuery3).GetTotalCount(); }));
            taskList.Add(Task.Factory.StartNew(() => { PricesTotalCount = _pricesDataSourceFactory(dataQuery2).GetTotalCount(); }));

            Task.WhenAll(taskList).GetAwaiter().GetResult();

            TotalCount = PricelistsTotalCount + AssignmentsTotalCount + PricesTotalCount;

        }

        public int GetTotalCount()
        {
            CalculateCounts();
            return TotalCount;
        }

        public virtual IEnumerable<IExportable> FetchNextPage()
        {


            var result = _pricelistDataSource.FetchNextPage();
            if (result.Count() == 0)
            {
                _pricelistAssignmentExportDataQuery.Take -= PricelistsTotalCount;
                _assignmentsDataSource.DataQuery = _pricelistAssignmentExportDataQuery;
                result = _assignmentsDataSource.FetchNextPage();
            }
            if (result.Count() == 0)
            {
                DataQuery.Skip = 0;
                DataQuery.Take = DataQuery.Take - AssignmentsTotalCount;
                result = _pricesDataSource.FetchNextPage();
            }
            CurrentPageNumber++;

            return result;
        }

    }
}
