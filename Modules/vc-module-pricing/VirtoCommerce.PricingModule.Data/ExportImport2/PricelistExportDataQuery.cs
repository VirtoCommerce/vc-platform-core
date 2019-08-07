using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model.Search;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class PricelistExportDataQuery : ExportDataQuery
    {
        public string[] Currencies { get; set; }

        public override SearchCriteriaBase CreateSearchCriteria()
        {
            return new PricelistSearchCriteria();
        }

        public override SearchCriteriaBase ToSearchCriteria()
        {
            var result = base.ToSearchCriteria();
            if (result is PricelistSearchCriteria pricelistSearchCriteria)
            {
                pricelistSearchCriteria.Currencies = Currencies;
            }

            return result;
        }

        public override ExportDataQuery FromSearchCriteria(SearchCriteriaBase searchCriteria)
        {
            var result = base.FromSearchCriteria(searchCriteria);
            if (searchCriteria is PricelistSearchCriteria pricelistSearchCriteria)
            {
                Currencies = pricelistSearchCriteria.Currencies;
            }

            return result;
        }
    }
}
