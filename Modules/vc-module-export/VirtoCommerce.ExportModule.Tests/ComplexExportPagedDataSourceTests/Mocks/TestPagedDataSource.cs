using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Data.Services;

namespace VirtoCommerce.ExportModule.Tests.ComplexExportPagedDataSourceTests.Mocks
{
    public class TestPagedDataSource : ExportPagedDataSource<TestExportDataQuery, TestSearchCriteria>
    {
        private readonly ITestSearchService _dataSource;

        public TestPagedDataSource(ITestSearchService dataSource, TestExportDataQuery dataQuery) : base(dataQuery)
        {
            _dataSource = dataSource;
        }

        protected override ExportableSearchResult FetchData(TestSearchCriteria searchCriteria)
        {
            var searchResult = _dataSource.SearchAsync(searchCriteria).GetAwaiter().GetResult();
            return new ExportableSearchResult { Results = searchResult.Results, TotalCount = searchResult.TotalCount };
        }
    }
}
