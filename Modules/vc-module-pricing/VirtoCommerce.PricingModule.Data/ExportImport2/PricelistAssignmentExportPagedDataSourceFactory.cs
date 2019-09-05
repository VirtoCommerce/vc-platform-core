using System;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.PricingModule.Core.Services;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class PricelistAssignmentExportPagedDataSourceFactory : IPagedDataSourceFactory
    {
        private readonly IPricingSearchService _searchService;
        private readonly IPricingService _pricingService;
        private readonly ICatalogService _catalogService;

        public PricelistAssignmentExportPagedDataSourceFactory(IPricingSearchService searchService, IPricingService pricingService, ICatalogService catalogService)
        {
            _searchService = searchService;
            _pricingService = pricingService;
            _catalogService = catalogService;
        }

        public virtual IPagedDataSource Create(ExportDataQuery dataQuery)
        {
            var pricelistAssignmentExportDataQuery = dataQuery as PricelistAssignmentExportDataQuery ?? throw new InvalidCastException($"Cannot cast dataQuery to type {typeof(PricelistAssignmentExportDataQuery)}");

            return new PricelistAssignmentExportPagedDataSource(_searchService, _pricingService, _catalogService, pricelistAssignmentExportDataQuery);
        }
    }
}
