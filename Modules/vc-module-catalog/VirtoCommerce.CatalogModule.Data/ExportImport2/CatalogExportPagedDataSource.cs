using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Data.Services;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class CatalogExportPagedDataSource : ExportPagedDataSource<CatalogExportDataQuery, CatalogSearchCriteria>
    {
        private readonly ICatalogSearchService _catalogSearchService;

        public CatalogExportPagedDataSource(ICatalogSearchService catalogSearchService, CatalogExportDataQuery dataQuery) : base(dataQuery)
        {
            _catalogSearchService = catalogSearchService;
        }

        protected override ExportableSearchResult FetchData(CatalogSearchCriteria searchCriteria)
        {
            var searchResult = _catalogSearchService.SearchCatalogsAsync(searchCriteria).GetAwaiter().GetResult();

            return new ExportableSearchResult
            {
                TotalCount = searchResult.TotalCount,
                Results = searchResult.Results.ToList<IExportable>(),
            };
        }

        protected override CatalogSearchCriteria BuildSearchCriteria(CatalogExportDataQuery exportDataQuery)
        {
            var result = base.BuildSearchCriteria(exportDataQuery);

            result.CatalogIds = exportDataQuery.CatalogIds;
            result.ResponseGroup = CatalogResponseGroup.Info.ToString();

            return result;
        }
    }
}
