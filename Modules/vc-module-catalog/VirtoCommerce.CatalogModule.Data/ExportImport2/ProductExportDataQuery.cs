using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class ProductExportDataQuery : CatalogFullExportDataQuery
    {
        public bool SearchInVariations { get; set; }
        public bool SearchInChildren { get; set; }
        public string[] CategoryIds { get; set; }
        public string ResponseGroup { get; set; }

        public override CatalogFullExportDataQuery FromOther(CatalogFullExportDataQuery other)
        {
            var result = base.FromOther(other);
            if(other.GetType() == typeof(CatalogFullExportDataQuery))
            {
                SearchInVariations = true;
                ResponseGroup = (ItemResponseGroup.Full & ~ItemResponseGroup.Variations).ToString();
            }
            return result;
        }
    }
}
