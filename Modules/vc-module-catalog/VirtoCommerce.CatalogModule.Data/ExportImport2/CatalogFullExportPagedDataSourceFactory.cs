using System;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class CatalogFullExportPagedDataSourceFactory : IPagedDataSourceFactory
    {
        private readonly CatalogExportPagedDataSourceFactory _catalogDataSourceFactory;
        private readonly CategoryExportPagedDataSourceFactory _categoryDataSourceFactory;
        private readonly PropertyExportPagedDataSourceFactory _propertyDataSourceFactory;
        private readonly PropertyDictionaryItemExportPagedDataSourceFactory _propertyDictionaryItemDataSourceFactory;
        private readonly ProductExportPagedDataSourceFactory _productDataSourceFactory;

        public CatalogFullExportPagedDataSourceFactory(CatalogExportPagedDataSourceFactory catalogDataSourceFactory,
            CategoryExportPagedDataSourceFactory categoryDataSourceFactory,
            PropertyExportPagedDataSourceFactory propertyDataSourceFactory,
            PropertyDictionaryItemExportPagedDataSourceFactory propertyDictionaryItemDataSourceFactory,
            ProductExportPagedDataSourceFactory productDataSourceFactory)
        {
            _catalogDataSourceFactory = catalogDataSourceFactory;
            _categoryDataSourceFactory = categoryDataSourceFactory;
            _propertyDataSourceFactory = propertyDataSourceFactory;
            _propertyDictionaryItemDataSourceFactory = propertyDictionaryItemDataSourceFactory;
            _productDataSourceFactory = productDataSourceFactory;
        }

        public IPagedDataSource Create(ExportDataQuery dataQuery)
        {
            var catalogFullExportDataQuery = dataQuery as CatalogFullExportDataQuery ?? throw new InvalidCastException($"Cannot cast dataQuery to type {typeof(CatalogFullExportDataQuery)}");

            return new CatalogFullExportPagedDataSource(
                _catalogDataSourceFactory,
                _categoryDataSourceFactory,
                _propertyDataSourceFactory,
                _propertyDictionaryItemDataSourceFactory,
                _productDataSourceFactory,
                catalogFullExportDataQuery);
        }
    }
}
