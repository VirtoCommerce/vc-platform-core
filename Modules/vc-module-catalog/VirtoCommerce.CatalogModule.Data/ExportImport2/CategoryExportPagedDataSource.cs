using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Data.Services;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class CategoryExportPagedDataSource : ExportPagedDataSource<CategoryExportDataQuery, CategorySearchCriteria>
    {
        private readonly ICategorySearchService _categorySearchService;

        public CategoryExportPagedDataSource(ICategorySearchService categorySearchService, CategoryExportDataQuery dataQuery) : base(dataQuery)
        {
            _categorySearchService = categorySearchService;
        }

        protected override ExportableSearchResult FetchData(CategorySearchCriteria searchCriteria)
        {
            var searchResult = _categorySearchService.SearchCategoriesAsync(searchCriteria).GetAwaiter().GetResult();

            return new ExportableSearchResult
            {
                TotalCount = searchResult.TotalCount,
                Results = searchResult.Results.ToList<IExportable>(),
            };
        }

        protected override CategorySearchCriteria BuildSearchCriteria(CategoryExportDataQuery exportDataQuery)
        {
            var result = base.BuildSearchCriteria(exportDataQuery);

            result.CatalogIds = exportDataQuery.CatalogIds;

            return result;
        }
    }
}
