using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Data.Services;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class PropertyDictionaryItemExportPagedDataSource : ExportPagedDataSource<PropertyDictionaryItemExportDataQuery, PropertyDictionaryItemSearchCriteria>
    {
        private readonly IPropertyDictionaryItemSearchService _propertyDictionaryItemSearchService;

        public PropertyDictionaryItemExportPagedDataSource(IPropertyDictionaryItemSearchService propertyDictionaryItemSearchService, PropertyDictionaryItemExportDataQuery dataQuery) : base(dataQuery)
        {
            _propertyDictionaryItemSearchService = propertyDictionaryItemSearchService;
        }

        protected override ExportableSearchResult FetchData(PropertyDictionaryItemSearchCriteria searchCriteria)
        {
            var searchResult = _propertyDictionaryItemSearchService.SearchAsync(searchCriteria).GetAwaiter().GetResult();

            return new ExportableSearchResult
            {
                TotalCount = searchResult.TotalCount,
                Results = searchResult.Results.ToList<IExportable>(),
            };
        }

        protected override PropertyDictionaryItemSearchCriteria BuildSearchCriteria(PropertyDictionaryItemExportDataQuery exportDataQuery)
        {
            var result = base.BuildSearchCriteria(exportDataQuery);

            result.CatalogIds = exportDataQuery.CatalogIds;

            return result;
        }
    }
}
