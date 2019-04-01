using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.PricingModule.Core.Model
{
    public class ProductPrice
    {
        public string ProductId { get; set; }

        public CatalogProduct Product { get; set; }

        /// <summary>
        /// List prices for the products. It includes tiered prices also. (Depending on the quantity, for example)
        /// </summary>
        public ICollection<Price> Prices { get; set; }
    }
}
