using System;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class PropertyExportPagedDataSourceFactory : IPagedDataSourceFactory
    {
        private readonly IPropertySearchService _propertySearchService;

        public PropertyExportPagedDataSourceFactory(IPropertySearchService propertySearchService)
        {
            _propertySearchService = propertySearchService;
        }

        public virtual IPagedDataSource Create(ExportDataQuery dataQuery)
        {
            var propertyExportDataQuery = dataQuery as PropertyExportDataQuery ?? throw new InvalidCastException($"Cannot cast dataQuery to type {typeof(PropertyExportDataQuery)}");

            return new PropertyExportPagedDataSource(_propertySearchService, propertyExportDataQuery);
        }
    }
}
