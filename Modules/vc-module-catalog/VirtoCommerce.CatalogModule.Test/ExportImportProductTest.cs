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
    public class ExportImportProductTest
    {
        [Fact]
        public void ProductFetchAll()
        {
            // Arrange
            var productExportPagedDataSource = InitProductExportPagedDataSource();
            // Act
            var fetchResult = productExportPagedDataSource.FetchNextPage();
            // Assert
            Assert.Equal(2, fetchResult.Count());
        }

        [Fact]
        public void CatalogFetchForSpecificIds()
        {
            // Arrange
            var productExportPagedDataSource = InitProductExportPagedDataSource();
            productExportPagedDataSource.DataQuery.ObjectIds = new[] { "7829d35f417e4dd98851f51322f32c23" };
            // Act
            var fetchResult = productExportPagedDataSource.FetchNextPage();
            // Assert
            Assert.Single(fetchResult);
        }


        private ProductExportPagedDataSource InitProductExportPagedDataSource()
        {
            var authorizationServicesMock = AuthMockHelper.AuthServicesMock(true);
            var productSearchServiceMock = new Mock<IProductSearchService>();
            productSearchServiceMock.Setup(x => x.SearchProductsAsync(It.IsAny<ProductSearchCriteria>())).ReturnsAsync(GetTestProductSearchResult());
            var productServiceMock = new Mock<IItemService>();
            productServiceMock.Setup(x => x.GetByIdsAsync(new[] { "7829d35f417e4dd98851f51322f32c23" }, null, null)).ReturnsAsync(GetTestProductByIdsResult());
            return new ProductExportPagedDataSource(
                productSearchServiceMock.Object,
                productServiceMock.Object,
                authorizationServicesMock.AuthorizationPolicyProvider,
                authorizationServicesMock.AuthorizationService,
                authorizationServicesMock.UserClaimsPrincipalFactory,
                authorizationServicesMock.UserManager)
            {
                DataQuery = new ProductExportDataQuery()
            };
        }

        private ProductSearchResult GetTestProductSearchResult()
        {
            return new ProductSearchResult()
            {
                TotalCount = 2,
                Results = new CatalogProduct[]
                {
                    new CatalogProduct(),
                    new CatalogProduct(){Id = "7829d35f417e4dd98851f51322f32c23" }
                }
            };
        }

        private CatalogProduct[] GetTestProductByIdsResult()
        {
            return new CatalogProduct[]
            {
                new CatalogProduct() { Id = "7829d35f417e4dd98851f51322f32c23" }
            };
        }
    }
}
