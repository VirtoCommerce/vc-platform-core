using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class ProductExportDataQuery : ExportDataQuery
    {
        public override SearchCriteriaBase CreateSearchCriteria()
        {
            return new ProductSearchCriteria();
        }
    }
}
