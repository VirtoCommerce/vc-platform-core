using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.ExportModule.Data.Services;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class CatalogFullExportPagedDataSource : ComplexExportPagedDataSource<CatalogFullExportDataQuery>
    {
        private readonly ICatalogSearchService _catalogSearchService;
        private readonly IProductSearchService _productSearchService;
        private readonly ICategorySearchService _categorySearchService;
        private readonly IPropertySearchService _propertySearchService;
        private readonly IProperyDictionaryItemSearchService _propertyDictionarySearchService;

        public CatalogFullExportPagedDataSource(ICatalogSearchService catalogSearchService,
            IProductSearchService productSearchService,
            ICategorySearchService categorySearchService,
            IPropertySearchService propertySearchService,
            IProperyDictionaryItemSearchService propertyDictionarySearchService,
            CatalogFullExportDataQuery dataQuery)
        : base(dataQuery)
        {
            _productSearchService = productSearchService;
            _categorySearchService = categorySearchService;
            _propertySearchService = propertySearchService;
            _propertyDictionarySearchService = propertyDictionarySearchService;
            _catalogSearchService = catalogSearchService;
        }

        protected override void InitDataSourceStates()
        {
            _exportDataSourceStates.AddRange(new ExportDataSourceState[]
            {
                new ExportDataSourceState
                {
                    SearchCriteria = new CatalogSearchCriteria { ResponseGroup = CatalogResponseGroup.Info.ToString(), CatalogIds = DataQuery.CatalogIds },
                    FetchFunc = async (x) =>
                    {
                        var searchResult = await _catalogSearchService.SearchCatalogsAsync((CatalogSearchCriteria)x.SearchCriteria);
                        x.TotalCount = searchResult.TotalCount;
                        x.Result = searchResult.Results;
                    }
                },
                new ExportDataSourceState
                {
                    SearchCriteria = new CategorySearchCriteria { CatalogIds = DataQuery.CatalogIds },
                    FetchFunc = async (x) =>
                    {
                        var searchResult = await _categorySearchService.SearchCategoriesAsync((CategorySearchCriteria)x.SearchCriteria);
                        x.TotalCount = searchResult.TotalCount;
                        x.Result = searchResult.Results;
                    }
                },
                new ExportDataSourceState
                {
                    SearchCriteria = new PropertySearchCriteria { CatalogIds = DataQuery.CatalogIds },
                    FetchFunc = async (x) =>
                    {
                        var searchResult = await _propertySearchService.SearchPropertiesAsync((PropertySearchCriteria)x.SearchCriteria);
                        x.TotalCount = searchResult.TotalCount;
                        x.Result = searchResult.Results;
                    }
                },
                new ExportDataSourceState
                {
                    SearchCriteria = new PropertyDictionaryItemSearchCriteria() { CatalogIds = DataQuery.CatalogIds },
                    FetchFunc = async (x) =>
                    {
                        var searchResult = await _propertyDictionarySearchService.SearchAsync((PropertyDictionaryItemSearchCriteria)x.SearchCriteria);
                        x.TotalCount = searchResult.TotalCount;
                        x.Result = searchResult.Results;
                    }
                },
                new ExportDataSourceState
                {
                    SearchCriteria = new ProductSearchCriteria() { CatalogIds = DataQuery.CatalogIds, SearchInVariations = true, ResponseGroup = (ItemResponseGroup.Full & ~ItemResponseGroup.Variations).ToString() },
                    FetchFunc = async (x) =>
                    {
                        var searchResult = await _productSearchService.SearchProductsAsync((ProductSearchCriteria)x.SearchCriteria);
                        x.TotalCount = searchResult.TotalCount;
                        x.Result = searchResult.Results;
                    }
                }
            });
        }
    }
}
