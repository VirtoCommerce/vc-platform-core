using System;
using VirtoCommerce.CatalogModule.Core.Model.Search;

namespace VirtoCommerce.CatalogModule.Web.Model
{
    public class SearchCriteria
    {
        public SearchCriteria()
        {
            Take = 20;
        }

        public string StoreId { get; set; }
        public SearchResponseGroup ResponseGroup { get; set; }
        public string Keyword { get; set; }

        /// <summary>
        /// Search  in all children categories for specified catalog or categories
        /// </summary>
        public bool SearchInChildren { get; set; }
        /// <summary>
        /// Also search in variations 
        /// </summary>
        public bool SearchInVariations { get; set; }

        public string CategoryId { get; set; }

        public string[] CategoryIds { get; set; }

        public string CatalogId { get; set; }

        public string[] CatalogIds { get; set; }

        public string LanguageCode { get; set; }

        /// <summary>
        /// Product or category code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Sorting expression property1:asc;property2:desc
        /// </summary>
        public string Sort { get; set; }

        //Hides direct linked categories in virtual category displayed only linked category content without itself
        public bool HideDirectLinkedCategories { get; set; }
        /// <summary>
        /// For filtration by specified properties values
        /// </summary>
        public PropertyValue[] PropertyValues { get; set; }

        public string Currency { get; set; }
        public decimal? StartPrice { get; set; }
        public decimal? EndPrice { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        /// <summary>
        /// All products have index date less that specified
        /// </summary>
        public DateTime? IndexDate { get; set; }

        public string PricelistId { get; set; }

        public string[] PricelistIds { get; set; }

        /// <summary>
        /// Gets or sets search terms collection
        /// Item format: name:value1,value2,value3
        /// </summary>
        public string[] Terms { get; set; }

        /// <summary>
        /// Gets or sets the facets collection
        /// Item format: name:value1,value2,value3
        /// </summary>
        public string[] Facets { get; set; }

        /// <summary>
        /// Category1/Category2
        /// </summary>
        public string Outline { get; set; }

        /// <summary>
        /// Search also in hidden categories and products
        /// </summary>
        public bool WithHidden { get; set; }

        /// <summary>
        /// Search only buyable products
        /// </summary>
        public bool? OnlyBuyable { get; set; }

        /// <summary>
        /// Search only inventory tracking products 
        /// </summary>
        public bool? OnlyWithTrackingInventory { get; set; }

        /// <summary>
        /// Search product with specified type
        /// </summary>
        public string ProductType { get; set; }

        /// <summary>
        /// Search product with specified types
        /// </summary>
        public string[] ProductTypes { get; set; }

        //Search by product vendor
        public string VendorId { get; set; }
        public string[] VendorIds { get; set; }

        public DateTime? StartDateFrom { get; set; }

    }
}
