using System.Collections.Generic;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.PricingModule.Web.Model
{
	public class ProductPrice
	{
  		public string ProductId { get; set; }

        // TODO: uncomment this property when Product will be available in VirtoCommerce.CatalogModule.Web.Model
        //public Product Product { get; set; }
        
        /// <summary>
        /// List prices for the products. It includes tiered prices also. (Depending on the quantity, for example)
        /// </summary>
		public ICollection<Price> Prices { get; set; }
	}
}
