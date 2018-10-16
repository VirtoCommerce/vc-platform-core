using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CatalogModule.Web.Model;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.PricingModule.Web.Model
{
	public class ProductPrice
	{
  		public string ProductId { get; set; }
		public Product Product { get; set; }
        
        /// <summary>
        /// List prices for the products. It includes tiered prices also. (Depending on the quantity, for example)
        /// </summary>
		public ICollection<Price> Prices { get; set; }
	}
}
