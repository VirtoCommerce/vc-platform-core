using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model.Search
{
    public class ProductSearchCriteria : SearchCriteriaBase
    {
        public string CatalogId { get; set; }
        public string CategoryId { get; set; }
        public IList<string> Skus { get; set; }

        /// <summary>
        /// Search product with specified types
        /// </summary>
        public string[] ProductTypes { get; set; }
    }
}
