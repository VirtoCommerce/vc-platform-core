using System;
using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.PricingModule.Core.Model
{
	public class PriceEvaluationContext : EvaluationContextBase
	{
		public string StoreId { get; set; } 
		public string CatalogId { get; set; }
		public string[] ProductIds { get; set; }
        // The ordered list of price-list IDs, evaluation logic will return only matched product prices from the first price-list from this list
        // To return all the prices found, simply set ReturnAllMatchedPrices to true
		public string[] PricelistIds { get; set; }
        //Set this flag to true for return all matched prices from all given pricelists 
	    public bool ReturnAllMatchedPrices { get; set; }
        public decimal Quantity { get; set; }
		public string CustomerId { get; set; }
		public string OrganizationId { get; set; }
		public DateTime? CertainDate { get; set; }
		public string Currency { get; set; }
	}
}
