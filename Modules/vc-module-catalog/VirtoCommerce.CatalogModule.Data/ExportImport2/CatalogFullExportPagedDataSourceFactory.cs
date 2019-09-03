using System;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class CatalogFullExportPagedDataSourceFactory : IPagedDataSourceFactory
    {
        private readonly ICatalogSearchService _catalogSearchService;
        private readonly IProductSearchService _productSearchService;
        private readonly ICategorySearchService _categorySearchService;
        private readonly IPropertySearchService _propertySearchService;
        private readonly IProperyDictionaryItemSearchService _propertyDictionarySearchService;

        public CatalogFullExportPagedDataSourceFactory(ICatalogSearchService catalogSearchService, IProductSearchService productSearchService, ICategorySearchService categorySearchService,
                          IPropertySearchService propertySearchService, IProperyDictionaryItemSearchService propertyDictionarySearchService)
        {
            _productSearchService = productSearchService;
            _categorySearchService = categorySearchService;
            _propertySearchService = propertySearchService;
            _propertyDictionarySearchService = propertyDictionarySearchService;
            _catalogSearchService = catalogSearchService;
        }

        public IPagedDataSource Create(ExportDataQuery dataQuery)
        {
            var catalogFullExportDataQuery = dataQuery as CatalogFullExportDataQuery ?? throw new InvalidCastException($"Cannot cast dataQuery to type {typeof(CatalogFullExportDataQuery)}");

            return new CatalogFullExportPagedDataSource(_catalogSearchService, _productSearchService, _categorySearchService, _propertySearchService, _propertyDictionarySearchService, catalogFullExportDataQuery);
        }
    }
}
