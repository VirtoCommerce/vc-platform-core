using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model.Search
{
    public class CatalogListEntrySearchCriteria : SearchCriteriaBase
    {
        public string Code { get; set; }
        /// <summary>
        /// Search by vendor
        /// </summary>
        public string VendorId { get; set; }

        private string[] _vendorIds;
        public string[] VendorIds
        {
            get
            {
                if (_vendorIds == null && !string.IsNullOrEmpty(VendorId))
                {
                    _vendorIds = new[] { VendorId };
                }
                return _vendorIds;
            }
            set
            {
                _vendorIds = value;
            }
        }
        /// <summary>
        /// Search product with specified type
        /// </summary>
        public string ProductType { get; set; }

        /// <summary>
        /// Search product with specified types
        /// </summary>
        private string[] _productTypes;
        public string[] ProductTypes
        {
            get
            {
                if (_productTypes == null && !string.IsNullOrEmpty(ProductType))
                {
                    _productTypes = new[] { ProductType };
                }
                return _productTypes;
            }
            set
            {
                _productTypes = value;
            }
        }

        public bool WithHidden { get; set; } = true;
        //Hides direct linked categories in virtual category displayed only linked category content without itself
        public bool HideDirectLinkedCategories { get; set; }

        /// <summary>
        /// Search  in all children categories for specified catalog or categories
        /// </summary>
        public bool SearchInChildren { get; set; }
        /// <summary>
        /// Also search in variations 
        /// </summary>
        public bool SearchInVariations { get; set; }

        public bool? OnlyBuyable { get; set; }
        public bool? OnlyWithTrackingInventory { get; set; }
        public string CatalogId { get; set; }

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
            set
            {
                _catalogIds = value;
            }

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
            set
            {
                _categoriesIds = value;
            }
        }

        public virtual void Normalize()
        {
            Keyword = Keyword.EmptyToNull();
            Sort = Sort.EmptyToNull();
            CatalogId = CatalogId.EmptyToNull();
            CategoryId = CategoryId.EmptyToNull();

            if (!string.IsNullOrEmpty(Keyword))
            {
                Keyword = Keyword.EscapeSearchTerm();
            }
        }

    }
}
