using System.Linq;
using Moq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CatalogModule.Data.ExportImport;
using VirtoCommerce.ExportModule.Test.MockHelpers;
using Xunit;

namespace VirtoCommerce.CatalogModule.Test
{
    public class ExportImport2Test
    {
        [Fact]
        public void CatalogFetchAll()
        {
            // Arrange
            var catalogExportPagedDataSource = InitCatalogExportPagedDataSource();
            // Act
            var fetchResult = catalogExportPagedDataSource.FetchNextPage();
            // Assert
            Assert.Equal(2, fetchResult.Count());
        }

        [Fact]
        public void CatalogFetchForSpecificIds()
        {
            // Arrange
            var catalogExportPagedDataSource = InitCatalogExportPagedDataSource();
            catalogExportPagedDataSource.DataQuery.ObjectIds = new[] { "7829d35f417e4dd98851f51322f32c23" };
            // Act
            var fetchResult = catalogExportPagedDataSource.FetchNextPage();
            // Assert
            Assert.Single(fetchResult);
        }

        private CatalogExportPagedDataSource InitCatalogExportPagedDataSource()
        {
            var authorizationServicesMock = AuthMockHelper.AuthServicesMock(true);
            var сatalogSearchServiceMock = new Mock<ICatalogSearchService>();
            сatalogSearchServiceMock.Setup(x => x.SearchCatalogsAsync(It.IsAny<CatalogSearchCriteria>())).ReturnsAsync(GetTestCatalogSearchResult());
            var сatalogServiceMock = new Mock<ICatalogService>();
            сatalogServiceMock.Setup(x => x.GetByIdsAsync(new string[] { "7829d35f417e4dd98851f51322f32c23" }, null)).ReturnsAsync(GetTestCatalogByIdsResult());
            return new CatalogExportPagedDataSource(
                сatalogSearchServiceMock.Object,
                сatalogServiceMock.Object,
                authorizationServicesMock.AuthorizationPolicyProvider,
                authorizationServicesMock.AuthorizationService,
                authorizationServicesMock.UserClaimsPrincipalFactory,
                authorizationServicesMock.UserManager)
            {
                DataQuery = new CatalogExportDataQuery()
            };
        }

        private CatalogSearchResult GetTestCatalogSearchResult()
        {
            return new CatalogSearchResult()
            {
                TotalCount = 2,
                Results = new Catalog[]
                {
                    new Catalog(),
                    new Catalog(){Id = "7829d35f417e4dd98851f51322f32c23" }
                }
            };
        }

        private Catalog[] GetTestCatalogByIdsResult()
        {
            return new Catalog[]
            {
                new Catalog() { Id = "7829d35f417e4dd98851f51322f32c23" }
            };
        }
    }
}
