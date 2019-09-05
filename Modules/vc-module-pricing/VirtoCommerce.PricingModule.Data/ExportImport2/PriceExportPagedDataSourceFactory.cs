using System;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.PricingModule.Core.Services;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class PriceExportPagedDataSourceFactory : IPagedDataSourceFactory
    {
        private readonly IPricingSearchService _searchService;
        private readonly IPricingService _pricingService;
        private readonly IItemService _itemService;

        public PriceExportPagedDataSourceFactory(IPricingSearchService searchService, IPricingService pricingService, IItemService itemService)
        {
            _searchService = searchService;
            _pricingService = pricingService;
            _itemService = itemService;
        }

        public virtual IPagedDataSource Create(ExportDataQuery dataQuery)
        {
            var priceExportDataQuery = dataQuery as PriceExportDataQuery ?? throw new InvalidCastException($"Cannot cast dataQuery to type {typeof(PriceExportDataQuery)}");

            return new PriceExportPagedDataSource(_searchService, _pricingService, _itemService, priceExportDataQuery);
        }
    }
}
