using System;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class CategoryExportPagedDataSourceFactory : IPagedDataSourceFactory
    {
        private readonly ICategorySearchService _categorySearchService;

        public CategoryExportPagedDataSourceFactory(ICategorySearchService categorySearchService)
        {
            _categorySearchService = categorySearchService;
        }

        public virtual IPagedDataSource Create(ExportDataQuery dataQuery)
        {
            var categoryExportDataQuery = dataQuery as CategoryExportDataQuery ?? throw new InvalidCastException($"Cannot cast dataQuery to type {typeof(CategoryExportDataQuery)}");

            return new CategoryExportPagedDataSource(_categorySearchService, categoryExportDataQuery);
        }
    }
}
