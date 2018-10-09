using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Shipping;
using VirtoCommerce.CoreModule.Core.Tax;

namespace VirtoCommerce.OrdersModule.Core.Model
{
	public class Shipment : OrderOperation, IHasTaxDetalization, ISupportCancellation, ITaxable, IHasDiscounts
	{
		public string OrganizationId { get; set; }
		public string OrganizationName { get; set; }

		public string FulfillmentCenterId { get; set; }
		public string FulfillmentCenterName { get; set; }

		public string EmployeeId { get; set; }
		public string EmployeeName { get; set; }

        /// <summary>
        /// Current shipment method code 
        /// </summary>
		public string ShipmentMethodCode { get; set; }

        /// <summary>
        /// Current shipment option code 
        /// </summary>
        public string ShipmentMethodOption { get; set; }

        /// <summary>
        ///  Shipment method contains additional shipment method information
        /// </summary>
        public ShippingMethod ShippingMethod { get; set; }

        public string CustomerOrderId { get; set; }
		public CustomerOrder CustomerOrder { get; set; }

		public ICollection<ShipmentItem> Items { get; set; } 

		public ICollection<ShipmentPackage> Packages { get; set; }

		public ICollection<PaymentIn> InPayments { get; set; }

		public string WeightUnit { get; set; }
		public decimal? Weight { get; set; }

		public string MeasureUnit { get; set; }
		public decimal? Height { get; set; }
		public decimal? Length { get; set; }
		public decimal? Width { get; set; }


        #region IHasDiscounts
        public ICollection<Discount> Discounts { get; set; }
        #endregion

        public Address DeliveryAddress { get; set; }

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

        public decimal TaxTotal { get; set; }

        public decimal TaxPercentRate { get; set; }

        #endregion



        #region ITaxDetailSupport Members

        public ICollection<TaxDetail> TaxDetails { get; set; }

		#endregion
	}
}
