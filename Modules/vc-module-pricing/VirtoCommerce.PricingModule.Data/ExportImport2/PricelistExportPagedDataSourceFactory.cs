using System;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.PricingModule.Core.Services;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class PricelistExportPagedDataSourceFactory : IPagedDataSourceFactory
    {
        private readonly IPricingSearchService _searchService;
        private readonly IPricingService _pricingService;

        public PricelistExportPagedDataSourceFactory(IPricingSearchService searchService, IPricingService pricingService)
        {
            _searchService = searchService;
            _pricingService = pricingService;
        }

        public virtual IPagedDataSource Create(ExportDataQuery dataQuery)
        {
            var pricelistExportDataQuery = dataQuery as PricelistExportDataQuery ?? throw new InvalidCastException($"Cannot cast dataQuery to type {typeof(PricelistExportDataQuery)}");

            return new PricelistExportPagedDataSource(_searchService, _pricingService, pricelistExportDataQuery);
        }
    }
}
