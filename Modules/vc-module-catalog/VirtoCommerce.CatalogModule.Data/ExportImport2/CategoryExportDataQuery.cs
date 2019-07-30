using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.ExportImport2
{
    public class CategoryExportDataQuery : ExportDataQuery
    {
        public string CatalogId { get; set; }
        public string CategoryId { get; set; }

        public override SearchCriteriaBase CreateSearchCriteria()
        {
            return new CategorySearchCriteria();
        }

        public override SearchCriteriaBase ToSearchCriteria()
        {
            var result = base.ToSearchCriteria();

            if (result is CategorySearchCriteria categorySearchCriteria)
            {
                categorySearchCriteria.CatalogId = CatalogId;
                categorySearchCriteria.CategoryId = CategoryId;
            }

            return result;
        }

        public override ExportDataQuery FromSearchCriteria(SearchCriteriaBase searchCriteria)
        {
            if (searchCriteria is CategorySearchCriteria pricesSearchCriteria)
            {
                CatalogId = pricesSearchCriteria.CatalogId;
                CategoryId = pricesSearchCriteria.CategoryId;
            }

            return base.FromSearchCriteria(searchCriteria);
        }
    }
}
