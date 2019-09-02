using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Data.ExportImport;
using VirtoCommerce.ExportModule.Core.Model;
using Xunit;

namespace VirtoCommerce.CatalogModule.Test
{
    public class ExportImport2Test
    {
        /// <summary>
        /// Tests cross-page walking on search services for Catalog and its dependencies
        /// </summary>
        /// <returns></returns>
        [Fact]
        public Task CatalogFullExportPagedDataSource_Paging_Valid()
        {
            var catalogSearchServiceMock = new Mock<ICatalogSearchService>();
            catalogSearchServiceMock.Setup(x => x.SearchCatalogsAsync(It.IsAny<CatalogSearchCriteria>())).ReturnsAsync((CatalogSearchCriteria y) =>
            {
                var searchResult = new string[] { "1", "2", "3" }.EmulateSearch<Catalog>(y);
                var catalogSearchResult = new CatalogSearchResult { Results = searchResult.Results, TotalCount = searchResult.TotalCount };
                return catalogSearchResult;
            }
            );

            var categorySearchServiceMock = new Mock<ICategorySearchService>();
            categorySearchServiceMock.Setup(x => x.SearchCategoriesAsync(It.IsAny<CategorySearchCriteria>())).ReturnsAsync((CategorySearchCriteria y) =>
            {
                var searchResult = new string[] { "4", "5", "6" }.EmulateSearch<Category>(y);
                var categorySearchResult = new CategorySearchResult { Results = searchResult.Results, TotalCount = searchResult.TotalCount };
                return categorySearchResult;
            }
            );

            var propertySearchServiceMock = new Mock<IPropertySearchService>();
            propertySearchServiceMock.Setup(x => x.SearchPropertiesAsync(It.IsAny<PropertySearchCriteria>())).ReturnsAsync((PropertySearchCriteria y) =>
            {
                var searchResult = new string[] { "7", "8", "9", "10", "11", "12", "13", "14" }.EmulateSearch<Property>(y);
                var propertySearchResult = new PropertySearchResult { Results = searchResult.Results, TotalCount = searchResult.TotalCount };
                return propertySearchResult;
            }
            );

            var propertyDictionaryItemSearchServiceMock = new Mock<IProperyDictionaryItemSearchService>();
            propertyDictionaryItemSearchServiceMock.Setup(x => x.SearchAsync(It.IsAny<PropertyDictionaryItemSearchCriteria>())).ReturnsAsync((PropertyDictionaryItemSearchCriteria y) =>
            {
                var searchResult = new string[] { "15", "16", "17" }.EmulateSearch<PropertyDictionaryItem>(y);
                var propertyDictionaryItemSearchResult = new PropertyDictionaryItemSearchResult { Results = searchResult.Results, TotalCount = searchResult.TotalCount };
                return propertyDictionaryItemSearchResult;
            }
            );

            var productSearchServiceMock = new Mock<IProductSearchService>();
            productSearchServiceMock.Setup(x => x.SearchProductsAsync(It.IsAny<ProductSearchCriteria>())).ReturnsAsync((ProductSearchCriteria y) =>
            {
                var searchResult = new string[] { "18", "19", "20", "21", "22" }.EmulateSearch<CatalogProduct>(y);
                var productSearchResult = new ProductSearchResult { Results = searchResult.Results, TotalCount = searchResult.TotalCount };
                return productSearchResult;
            }
            );

            var catalogDataQuery = new CatalogFullExportDataQuery();

            var catalogDataSource = new CatalogFullExportPagedDataSource(
                catalogSearchServiceMock.Object,
                productSearchServiceMock.Object,
                categorySearchServiceMock.Object,
                propertySearchServiceMock.Object,
                propertyDictionaryItemSearchServiceMock.Object,
                catalogDataQuery
                )
            { PageSize = 5 };

            var fetchResult = new List<IEnumerable<IExportable>>();
            while (catalogDataSource.Fetch())
            {
                fetchResult.Add(catalogDataSource.Items);
            }

            var result = string.Join(',', fetchResult.Select(x => string.Join(',', x.Select(y => y.Id)))); // It's for simplifying result watching 

            Assert.Equal("1,2,3,4,5", string.Join(',', fetchResult[0].Select(x => x.Id)));
            Assert.Equal("6,7,8,9,10", string.Join(',', fetchResult[1].Select(x => x.Id)));
            Assert.Equal("11,12,13,14,15", string.Join(',', fetchResult[2].Select(x => x.Id)));
            Assert.Equal("16,17,18,19,20", string.Join(',', fetchResult[3].Select(x => x.Id)));
            Assert.Equal("21,22", string.Join(',', fetchResult[4].Select(x => x.Id)));

            return Task.CompletedTask;
        }
    }
}
