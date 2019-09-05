using System;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class CatalogExportPagedDataSourceFactory : IPagedDataSourceFactory
    {
        private readonly ICatalogSearchService _catalogSearchService;

        public CatalogExportPagedDataSourceFactory(ICatalogSearchService catalogSearchService)
        {
            _catalogSearchService = catalogSearchService;
        }

        public virtual IPagedDataSource Create(ExportDataQuery dataQuery)
        {
            var catalogExportDataQuery = dataQuery as CatalogExportDataQuery ?? throw new InvalidCastException($"Cannot cast dataQuery to type {typeof(CatalogExportDataQuery)}");

            return new CatalogExportPagedDataSource(_catalogSearchService, catalogExportDataQuery);
        }
    }
}
