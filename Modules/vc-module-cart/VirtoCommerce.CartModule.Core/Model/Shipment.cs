using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Tax;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.CartModule.Core.Model
{
	public class Shipment : AuditableEntity, IHasTaxDetalization, ITaxable, IHasDiscounts, IHasDynamicProperties
    {
		public string ShipmentMethodCode { get; set; }
        public string ShipmentMethodOption { get; set; }
        public string FulfillmentCenterId { get; set; }
        public string FulfillmentCenterName { get; set; }
        public string  WarehouseLocation { get; set; }

		public string Currency { get; set; }
		public decimal? VolumetricWeight { get; set; }

		public string WeightUnit { get; set; }
		public decimal? Weight { get; set; }

		public string MeasureUnit { get; set; }
		public decimal? Height { get; set; }
		public decimal? Length { get; set; }
		public decimal? Width { get; set; }

		public virtual decimal Price { get; set; }

        public virtual decimal PriceWithTax { get; set; }

        public virtual decimal Total { get; set; }

        public virtual decimal TotalWithTax { get; set; }

        public virtual decimal DiscountAmount { get; set; }
        public virtual decimal DiscountAmountWithTax { get; set; }

        //Any extra Fee 
        public virtual decimal Fee { get; set; }

        public virtual decimal FeeWithTax { get; set; }


        #region ITaxable Members

        /// <summary>
        /// Tax category or type
        /// </summary>
        public string TaxType { get; set; }

        public decimal? TaxTotal { get; set; }

        public decimal? TaxPercentRate { get; set; }

        #endregion


        public Address DeliveryAddress { get; set; }

		public ICollection<ShipmentItem> Items { get; set; }


        #region IHasDiscounts
        public ICollection<Discount> Discounts { get; set; }
        #endregion

        #region IHaveTaxDetalization Members
        public ICollection<TaxDetail> TaxDetails { get; set; }
        #endregion

        #region IHasDynamicProperties Members
        public string ObjectType { get; set; }
        public ICollection<DynamicObjectProperty> DynamicProperties { get; set; }
        #endregion

    }
}
