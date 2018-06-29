namespace VirtoCommerce.CoreModule.Core.Model.Shipping
{
	public class ShippingRate
	{
    	/// <summary>
		/// Shipping option name or identifier
		/// </summary>
		public string OptionName { get; set; }

		/// <summary>
		/// Shipping option description
		/// </summary>
		public string OptionDescription { get; set; }

		public decimal Rate { get; set; }
        public decimal RateWithTax { get; set; }

		public string Currency { get; set; }

        public decimal DiscountAmount { get; set; }
        public decimal DiscountAmountWithTax { get; set; }

        public ShippingMethod ShippingMethod { get; set; }
	}
}
