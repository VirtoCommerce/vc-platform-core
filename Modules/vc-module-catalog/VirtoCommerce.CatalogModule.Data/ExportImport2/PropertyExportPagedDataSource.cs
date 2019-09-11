using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Data.Services;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class PropertyExportPagedDataSource : ExportPagedDataSource<PropertyExportDataQuery, PropertySearchCriteria>
    {
        private readonly IPropertySearchService _propertySearchService;

        public PropertyExportPagedDataSource(IPropertySearchService propertySearchService, PropertyExportDataQuery dataQuery) : base(dataQuery)
        {
            _propertySearchService = propertySearchService;
        }

        protected override ExportableSearchResult FetchData(PropertySearchCriteria searchCriteria)
        {
            var searchResult = _propertySearchService.SearchPropertiesAsync(searchCriteria).GetAwaiter().GetResult();

            return new ExportableSearchResult
            {
                TotalCount = searchResult.TotalCount,
                Results = searchResult.Results.ToList<IExportable>(),
            };
        }

        protected override PropertySearchCriteria BuildSearchCriteria(PropertyExportDataQuery exportDataQuery)
        {
            var result = base.BuildSearchCriteria(exportDataQuery);

            result.CatalogIds = exportDataQuery.CatalogIds;

            return result;
        }
    }
}
