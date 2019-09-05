using System;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class PropertyDictionaryItemExportPagedDataSourceFactory : IPagedDataSourceFactory
    {
        private readonly IPropertyDictionaryItemSearchService _propertyDictionaryItemSearchService;

        public PropertyDictionaryItemExportPagedDataSourceFactory(IPropertyDictionaryItemSearchService propertyDictionaryItemSearchService)
        {
            _propertyDictionaryItemSearchService = propertyDictionaryItemSearchService;
        }

        public IPagedDataSource Create(ExportDataQuery dataQuery)
        {
            var propertyDictionaryItemExportDataQuery = dataQuery as PropertyDictionaryItemExportDataQuery ?? throw new InvalidCastException($"Cannot cast dataQuery to type {typeof(PropertyDictionaryItemExportDataQuery)}");

            return new PropertyDictionaryItemExportPagedDataSource(_propertyDictionaryItemSearchService, propertyDictionaryItemExportDataQuery);
        }
    }
}
