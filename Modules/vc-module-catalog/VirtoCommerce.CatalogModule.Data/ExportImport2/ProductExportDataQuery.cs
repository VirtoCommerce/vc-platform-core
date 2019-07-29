using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class ProductExportDataQuery : ExportDataQuery
    {
        public string CatalogId { get; set; }
        public string CategoryId { get; set; }
        public IList<string> Skus { get; set; }
        public bool SearchInVariations { get; set; }
        public string[] ProductTypes { get; set; }
        public override SearchCriteriaBase CreateSearchCriteria()
        {
            return new ProductSearchCriteria();
        }

        public override SearchCriteriaBase ToSearchCriteria()
        {
            var result = base.ToSearchCriteria();

            if (result is ProductSearchCriteria productSearchCriteria)
            {
                productSearchCriteria.CatalogId = CatalogId;
                productSearchCriteria.CategoryId = CategoryId;
                productSearchCriteria.ProductTypes = ProductTypes;
                productSearchCriteria.SearchInVariations = SearchInVariations;
                productSearchCriteria.Skus = Skus;
            }

            return result;

        }

        public override ExportDataQuery FromSearchCriteria(SearchCriteriaBase searchCriteria)
        {
            if (searchCriteria is ProductSearchCriteria productSearchCriteria)
            {
                CatalogId = productSearchCriteria.CatalogId;
                CategoryId = productSearchCriteria.CategoryId;
                ProductTypes = productSearchCriteria.ProductTypes;
                SearchInVariations = productSearchCriteria.SearchInVariations;
                Skus = productSearchCriteria.Skus;
            }

            return base.FromSearchCriteria(searchCriteria);
        }
    }
}
