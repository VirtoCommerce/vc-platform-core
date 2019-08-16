using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.ExportImport
{
    public class PricelistAssignmentExportDataQuery : ExportDataQuery
    {
        public string[] PriceListIds { get; set; }
        public string[] CatalogIds { get; set; }
    }
}
