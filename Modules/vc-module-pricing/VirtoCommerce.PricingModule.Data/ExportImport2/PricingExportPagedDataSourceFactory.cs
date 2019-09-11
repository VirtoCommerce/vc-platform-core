using System;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Services;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class PricingExportPagedDataSourceFactory : IPricingExportPagedDataSourceFactory
    {
        private readonly IPricingSearchService _searchService;
        private readonly IPricingService _pricingService;
        private readonly IItemService _itemService;
        private readonly ICatalogService _catalogService;

        public PricingExportPagedDataSourceFactory(IPricingSearchService searchService, IPricingService pricingService, IItemService itemService, ICatalogService catalogService)
        {
            _searchService = searchService;
            _pricingService = pricingService;
            _itemService = itemService;
            _catalogService = catalogService;
        }

        public virtual IPagedDataSource Create(ExportDataQuery dataQuery)
        {
            IPagedDataSource result = null;

            if (dataQuery is PriceExportDataQuery priceExportDataQuery)
            {
                result = new PriceExportPagedDataSource(_searchService, _pricingService, _itemService, priceExportDataQuery);
            }
            else if (dataQuery is PricelistAssignmentExportDataQuery pricelistAssignmentExportDataQuery)
            {
                result = new PricelistAssignmentExportPagedDataSource(_searchService, _pricingService, _catalogService, pricelistAssignmentExportDataQuery);
            }
            else if (dataQuery is PricelistExportDataQuery pricelistExportDataQuery)
            {
                result = new PricelistExportPagedDataSource(_searchService, _pricingService, pricelistExportDataQuery);
            }

            return result ?? throw new ArgumentException($"Unsupported export query type: {dataQuery.GetType().Name}");
        }
    }
}
