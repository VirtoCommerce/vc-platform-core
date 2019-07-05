using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model.Search;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class PricelistExportDataQuery : ExportDataQuery
    {
        public PricelistExportDataQuery()
        {
            IncludedProperties = new[]
            {
                "Id", "Name", "Currency", "Assignments.Id", "Assignments.CatalogId", "Assignments.Name", "Assignments.Priority", "Assignments.ConditionExpression","Assignments.PredicateVisualTreeSerialized", "Prices.Id", "Prices.Currency", "Prices.ProductId", "Prices.Sale", "Prices.List", "Prices.MinQuantity","Prices.StartDate", "Prices.EndDate", "Prices.EffectiveValue"
            };
        }

        public override SearchCriteriaBase CreateSearchCriteria()
        {
            return new PricelistSearchCriteria();
        }
    }
}
