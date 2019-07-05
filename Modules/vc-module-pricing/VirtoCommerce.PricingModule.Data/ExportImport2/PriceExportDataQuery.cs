using System;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model.Search;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class PriceExportDataQuery : ExportDataQuery
    {
        public PriceExportDataQuery()
        {
            IncludedProperties = new[] { "Id", "PricelistId", "Currency", "ProductId", "Sale", "List", "MinQuantity", "StartDate", "EndDate", "EffectiveValue" };
        }

        public string[] PriceListIds { get; set; }

        public string[] ProductIds { get; set; }

        public DateTime? ModifiedSince { get; set; }

        public override SearchCriteriaBase CreateSearchCriteria()
        {
            return new PricesSearchCriteria();
        }

        public override SearchCriteriaBase ToSearchCriteria()
        {
            var result = base.ToSearchCriteria();

            if (result is PricesSearchCriteria pricesSearchCriteria)
            {
                pricesSearchCriteria.PriceListIds = PriceListIds;
                pricesSearchCriteria.ProductIds = ProductIds;
                pricesSearchCriteria.ModifiedSince = ModifiedSince;
            }

            return result;
        }

        public override ExportDataQuery FromSearchCriteria(SearchCriteriaBase searchCriteria)
        {
            if (searchCriteria is PricesSearchCriteria pricesSearchCriteria)
            {
                PriceListIds = pricesSearchCriteria.PriceListIds;
                ProductIds = pricesSearchCriteria.ProductIds;
                ModifiedSince = pricesSearchCriteria.ModifiedSince;

            }

            return base.FromSearchCriteria(searchCriteria);
        }
    }
}
