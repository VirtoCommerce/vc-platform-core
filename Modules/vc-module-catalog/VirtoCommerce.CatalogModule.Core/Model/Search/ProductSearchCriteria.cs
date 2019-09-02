using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model.Search
{
    public class ProductSearchCriteria : SearchCriteriaBase
    {
        public string CatalogId;
        private string[] _catalogIds;
        public string[] CatalogIds
        {
            get
            {
                if (_catalogIds == null && !string.IsNullOrEmpty(CatalogId))
                {
                    _catalogIds = new[] { CatalogId };
                }
                return _catalogIds;
            }
            set => _catalogIds = value;
        }
        public string CategoryId { get; set; }
        private string[] _categoriesIds;
        public string[] CategoryIds
        {
            get
            {
                if (_categoriesIds == null && !string.IsNullOrEmpty(CategoryId))
                {
                    _categoriesIds = new[] { CategoryId };
                }
                return _categoriesIds;
            }
            set => _categoriesIds = value;
        }
        public IList<string> Skus { get; set; }
        /// <summary>
        /// Include product variations in result
        /// </summary>
        public bool SearchInVariations { get; set; }
        /// <summary>
        /// Search product with specified types
        /// </summary>
        public string[] ProductTypes { get; set; }
        public bool SearchInChildren { get; set; }
    }
}
