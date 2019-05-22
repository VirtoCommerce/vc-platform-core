using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ShippingModule.Core.Model
{
    public class ShippingRate : ValueObject
    {
        /// <summary>
        /// Shipping option name or identifier
        /// </summary>
        public string OptionName { get; set; }

        public decimal Rate { get; set; }
        public decimal RateWithTax { get; set; }

        public string Currency { get; set; }

        public decimal DiscountAmount { get; set; }
        public decimal DiscountAmountWithTax { get; set; }

        public ShippingMethod ShippingMethod { get; set; }
    }
}
