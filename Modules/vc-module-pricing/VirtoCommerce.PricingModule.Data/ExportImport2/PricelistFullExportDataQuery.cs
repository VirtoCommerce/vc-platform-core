using System.Linq;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model.Search;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class PricelistFullExportDataQuery : ExportDataQuery
    {
        public PricelistFullExportDataQuery()
        {
            IncludedColumns = new[]
            {
                "Id", "Name", "Currency", "Description",
                "Assignments.Id", "Assignments.CatalogId", "Assignments.Name", "Assignments.Priority", "Assignments.ConditionExpression","Assignments.PredicateVisualTreeSerialized",
                "Prices.Id", "Prices.Currency", "Prices.ProductId", "Prices.Sale", "Prices.List", "Prices.MinQuantity","Prices.StartDate", "Prices.EndDate"
            }
            .Select(x => new ExportedTypeColumnInfo()
            {
                Name = x,
                ExportName = x,
            })
            .ToArray();
        }

        public override SearchCriteriaBase CreateSearchCriteria()
        {
            return new PricelistSearchCriteria();
        }
    }
}
