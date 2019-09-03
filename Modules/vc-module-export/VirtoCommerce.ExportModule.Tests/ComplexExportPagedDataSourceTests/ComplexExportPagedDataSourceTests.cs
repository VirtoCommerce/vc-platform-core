using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Tests.ComplexExportPagedDataSourceTests.Mocks;
using Xunit;

namespace VirtoCommerce.ExportModule.Tests.ComplexExportPagedDataSourceTests
{
    public class ComplexExportPagedDataSourceTests
    {
        /// <summary>
        /// Tests cross-page walking on search services for Catalog and its dependencies
        /// </summary>
        /// <returns></returns>
        [Fact]
        public Task CatalogFullExportPagedDataSource_Paging_Valid()
        {
            // Arrange
            var service1Mock = new Mock<IComplexDataSearchService>();
            service1Mock.Setup(x => x.SearchAsync(It.IsAny<ComplexSearchCriteria>())).ReturnsAsync((ComplexSearchCriteria y) =>
            {
                var searchResult = new string[] { "1", "2", "3" }.EmulateSearch<ComplexSearchEntity>(y);
                var complexSearchResult = new ComplexSearchResult { Results = searchResult.Results, TotalCount = searchResult.TotalCount };
                return complexSearchResult;
            }
            );

            var service2Mock = new Mock<IComplexDataSearchService>();
            service2Mock.Setup(x => x.SearchAsync(It.IsAny<ComplexSearchCriteria>())).ReturnsAsync((ComplexSearchCriteria y) =>
            {
                var searchResult = new string[] { "4", "5", "6" }.EmulateSearch<ComplexSearchEntity>(y);
                var complexSearchResult = new ComplexSearchResult { Results = searchResult.Results, TotalCount = searchResult.TotalCount };
                return complexSearchResult;
            }
            );

            var service3Mock = new Mock<IComplexDataSearchService>();
            service3Mock.Setup(x => x.SearchAsync(It.IsAny<ComplexSearchCriteria>())).ReturnsAsync((ComplexSearchCriteria y) =>
            {
                var searchResult = new string[] { "7" }.EmulateSearch<ComplexSearchEntity>(y);
                var complexSearchResult = new ComplexSearchResult { Results = searchResult.Results, TotalCount = searchResult.TotalCount };
                return complexSearchResult;
            }
            );

            var service4Mock = new Mock<IComplexDataSearchService>();
            service4Mock.Setup(x => x.SearchAsync(It.IsAny<ComplexSearchCriteria>())).ReturnsAsync((ComplexSearchCriteria y) =>
            {
                var searchResult = new string[] { "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20" }.EmulateSearch<ComplexSearchEntity>(y);
                var complexSearchResult = new ComplexSearchResult { Results = searchResult.Results, TotalCount = searchResult.TotalCount };
                return complexSearchResult;
            }
            );

            var service5Mock = new Mock<IComplexDataSearchService>();
            service5Mock.Setup(x => x.SearchAsync(It.IsAny<ComplexSearchCriteria>())).ReturnsAsync((ComplexSearchCriteria y) =>
            {
                var searchResult = new string[] { "21", "22" }.EmulateSearch<ComplexSearchEntity>(y);
                var complexSearchResult = new ComplexSearchResult { Results = searchResult.Results, TotalCount = searchResult.TotalCount };
                return complexSearchResult;
            }
            );

            var catalogDataQuery = new ComplexExportDataQuery();

            var catalogDataSource = new ComplexExportPagedDataSource(
                service1Mock.Object,
                service2Mock.Object,
                service3Mock.Object,
                service4Mock.Object,
                service5Mock.Object,
                catalogDataQuery
                )
            { PageSize = 5 };

            // Act
            var fetchResult = new List<IEnumerable<IExportable>>();
            while (catalogDataSource.Fetch())
            {
                fetchResult.Add(catalogDataSource.Items);
            }

            // Assert

            // It's for simplifying result watching 
            var result = string.Join(',', fetchResult.Select(x => string.Join(',', x.Select(y => y.Id))));

            Assert.Equal("1,2,3,4,5", string.Join(',', fetchResult[0].Select(x => x.Id)));
            Assert.Equal("6,7,8,9,10", string.Join(',', fetchResult[1].Select(x => x.Id)));
            Assert.Equal("11,12,13,14,15", string.Join(',', fetchResult[2].Select(x => x.Id)));
            Assert.Equal("16,17,18,19,20", string.Join(',', fetchResult[3].Select(x => x.Id)));
            Assert.Equal("21,22", string.Join(',', fetchResult[4].Select(x => x.Id)));

            return Task.CompletedTask;
        }
    }
}
