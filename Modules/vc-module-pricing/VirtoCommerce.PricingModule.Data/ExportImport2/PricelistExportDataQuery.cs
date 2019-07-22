using System.Linq;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model.Search;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class PricelistExportDataQuery : ExportDataQuery
    {
        public PricelistExportDataQuery()
        {
            IncludedColumns = new[]
            {
                "Id", "Name", "Currency", "Description",
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