using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class PricelistExportDataQuery : ExportDataQuery
    {
        public string[] Currencies { get; set; }
    }
}
