using System;
using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Model.Payment;
using VirtoCommerce.Domain.Commerce.Model;

namespace VirtoCommerce.OrderModule.Core.Model
{
	public class PaymentIn : OrderOperation, IHaveTaxDetalization, ITaxable, IHasDiscounts
    {
		public string Purpose { get; set; }
        /// <summary>
        /// Payment method (gateway) code
        /// </summary>
		public string GatewayCode { get; set; }
        /// <summary>
        /// Payment method contains additional payment method information
        /// </summary>
        public PaymentMethod PaymentMethod { get; set; }
		public string OrganizationId { get; set; }
		public string OrganizationName { get; set; }

		public string CustomerId { get; set; }
		public string CustomerName { get; set; }

		public DateTime? IncomingDate { get; set; }
		public string OuterId { get; set; }
		public Address BillingAddress { get; set; }

		public PaymentStatus PaymentStatus { get; set; }
		public DateTime? AuthorizedDate { get; set; }
		public DateTime? CapturedDate { get; set; }
		public DateTime? VoidedDate { get; set; }

        public ProcessPaymentResult ProcessPaymentResult { get; set; }

        //the self cost of the payment method
        public virtual decimal Price { get; set; }
        public virtual decimal PriceWithTax { get; set; }

        public virtual decimal Total { get; set; }

        public virtual decimal TotalWithTax { get; set; }

        public virtual decimal DiscountAmount { get; set; }
        public virtual decimal DiscountAmountWithTax { get; set; }


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


        #region IHasDiscounts
        public ICollection<Discount> Discounts { get; set; }
        #endregion

        public ICollection<PaymentGatewayTransaction> Transactions { get; set; }
    }
}
