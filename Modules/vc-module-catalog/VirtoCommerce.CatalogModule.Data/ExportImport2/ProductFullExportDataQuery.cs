using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class ProductFullExportDataQuery : CatalogFullExportDataQuery
    {
        public bool SearchInVariations { get; set; }
        public bool SearchInChildren { get; set; }
        public string[] CategoryIds { get; set; }
        public string ResponseGroup { get; set; }

        public override CatalogFullExportDataQuery FromOther(CatalogFullExportDataQuery other)
        {
            var result = base.FromOther(other);
            if (other.GetType() == typeof(CatalogFullExportDataQuery))
            {
                SearchInVariations = true;
                ResponseGroup = (ItemResponseGroup.Full & ~ItemResponseGroup.Variations).ToString();
            }
            return result;
        }

        public virtual ProductExportDataQuery ToProductExportDataQuery()
        {
            var result = AbstractTypeFactory<ExportDataQuery>.TryCreateInstance<ProductExportDataQuery>();

            result.CatalogIds = CatalogIds;
            result.CategoryIds = CategoryIds;
            result.IncludedProperties = IncludedProperties;
            result.Keyword = Keyword;
            result.ObjectIds = ObjectIds;
            result.ResponseGroup = ResponseGroup;
            result.SearchInChildren = SearchInChildren;
            result.SearchInVariations = SearchInVariations;
            result.Skip = Skip;
            result.Sort = Sort;
            result.Take = Take;
            result.LoadImageBinaries = true;

            return result;
        }
    }
}
