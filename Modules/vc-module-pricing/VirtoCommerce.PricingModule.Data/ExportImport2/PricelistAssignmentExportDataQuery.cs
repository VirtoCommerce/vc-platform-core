using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model.Search;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class PricelistAssignmentExportDataQuery : ExportDataQuery
    {
        public string[] PriceListIds { get; set; }
        public string[] CatalogIds { get; set; }
        public override SearchCriteriaBase CreateSearchCriteria()
        {
            return new PricelistAssignmentsSearchCriteria();
        }

        public override SearchCriteriaBase ToSearchCriteria()
        {
            var result = base.ToSearchCriteria();
            if (result is PricelistAssignmentsSearchCriteria pricelistAssignmentsSearchCriteria)
            {
                pricelistAssignmentsSearchCriteria.PriceListIds = PriceListIds;
                pricelistAssignmentsSearchCriteria.CatalogIds = CatalogIds;
            }

            return result;
        }

        public override ExportDataQuery FromSearchCriteria(SearchCriteriaBase searchCriteria)
        {
            if (searchCriteria is PricelistAssignmentsSearchCriteria pricelistAssignmentsSearchCriteria)
            {
                PriceListIds = pricelistAssignmentsSearchCriteria.PriceListIds;
                CatalogIds = pricelistAssignmentsSearchCriteria.CatalogIds;
            }

            return base.FromSearchCriteria(searchCriteria);
        }
    }
}
