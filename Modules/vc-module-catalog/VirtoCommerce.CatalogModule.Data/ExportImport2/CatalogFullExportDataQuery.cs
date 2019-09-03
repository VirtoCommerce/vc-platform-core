using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class CatalogFullExportDataQuery : ExportDataQuery
    {
        public string[] CatalogIds { get; set; }
    }
}
