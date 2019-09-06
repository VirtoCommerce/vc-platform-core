using System.Linq;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Data.Extensions;
using VirtoCommerce.ExportModule.Tests.ComplexExportPagedDataSourceTests.Mocks;
using Xunit;

namespace VirtoCommerce.ExportModule.Tests.ComplexExportPagedDataSourceTests
{
    public class MultiplePagedDataSourcesTests
    {
        [Theory]
        [InlineData(0, 5, "1,2,3,4,5")]
        [InlineData(5, 5, "6,7,8,9,10")]
        [InlineData(10, 5, "11,12,13,14,15")]
        [InlineData(15, 5, "16,17,18,19,20")]
        [InlineData(20, 5, "21,22")]
        [InlineData(0, 0, "")]
        [InlineData(1, 0, "")]
        [InlineData(0, 1, "1")]
        [InlineData(21, 1, "22")]
        public Task MultipleDataSources_Paging_Valid(int skip, int take, string expectedResult)
        {
            // Arrange
            var dataSources = new IPagedDataSource[] {
                new TestPagedDataSource(CreatedSearchServiceMock(new string[] { "1", "2", "3"  }).Object, new TestExportDataQuery()),
                new TestPagedDataSource(CreatedSearchServiceMock(new string[] { "4", "5", "6" }).Object, new TestExportDataQuery()),
                new TestPagedDataSource(CreatedSearchServiceMock(new string[] { "7" }).Object, new TestExportDataQuery()),
                new TestPagedDataSource(CreatedSearchServiceMock(new string[] { "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20" }).Object, new TestExportDataQuery()),
                new TestPagedDataSource(CreatedSearchServiceMock(new string[] { "21", "22" }).Object, new TestExportDataQuery()),
            };

            // Act
            var items = dataSources.GetItems(skip, take);
            var result = string.Join(',', items.Select(x => x.Id));

            // Assert
            Assert.Equal(expectedResult, result);

            return Task.CompletedTask;
        }

        [Theory]
        [InlineData(new int[] { }, 0)]
        [InlineData(new int[] { 1 }, 1)]
        [InlineData(new int[] { 1, 2 }, 3)]
        public Task MultipleDataSources_TotalCount_Valid(int[] counts, int expectedResult)
        {
            // Arrange
            var dataSources = counts.Select(x => new TestPagedDataSource(CreatedSearchServiceMock(new string[x]).Object, new TestExportDataQuery()));

            // Act
            var result = dataSources.Sum(x => x.GetTotalCount());

            // Assert
            Assert.Equal(expectedResult, result);

            return Task.CompletedTask;
        }

        private static Mock<ITestSearchService> CreatedSearchServiceMock(params string[] data)
        {
            var result = new Mock<ITestSearchService>();
            result.Setup(x => x.SearchAsync(It.IsAny<TestSearchCriteria>())).ReturnsAsync((TestSearchCriteria y) =>
            {
                var searchResult = data.EmulateSearch<TestSearchEntity>(y);
                return new ExportableSearchResult { Results = searchResult.Results, TotalCount = searchResult.TotalCount };
            });
            return result;
        }
    }
}
