using System.Collections.Generic;
using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class ProductExportDataQuery : ExportDataQuery
    {
        public string CatalogId { get; set; }
        public string CategoryId { get; set; }
        public IList<string> Skus { get; set; }
        public bool SearchInVariations { get; set; }
        public string[] ProductTypes { get; set; }
    }
}
