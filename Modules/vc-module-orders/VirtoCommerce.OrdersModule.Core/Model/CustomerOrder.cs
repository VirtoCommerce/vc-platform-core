using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Tax;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.OrdersModule.Core.Model
{
    public class CustomerOrder : OrderOperation, IHasTaxDetalization, ISupportSecurityScopes, ITaxable, IHasLanguage, IHasDiscounts
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string ChannelId { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }

        /// <summary>
        /// The order identifier in an external system.
        /// </summary>
        public string OuterId { get; set; }

        /// <summary>
        /// The basis shopping cart id of which the order was created
        /// </summary>
        public string ShoppingCartId { get; set; }
    
        /// <summary>
        /// Flag determines that the order is the prototype
        /// </summary>
        public bool IsPrototype { get; set; }
        /// <summary>
        /// Number for subscription  associated with this order
        /// </summary>
        public string SubscriptionNumber { get; set; }
        /// <summary>
        /// Identifier for subscription  associated with this order
        /// </summary>
        public string SubscriptionId { get; set; }
 
		public ICollection<Address> Addresses { get; set; }
		public ICollection<PaymentIn> InPayments { get; set; }

		public ICollection<LineItem> Items { get; set; }
		public ICollection<Shipment> Shipments { get; set; }


        #region IHasDiscounts
        public ICollection<Discount> Discounts { get; set; }
        #endregion

        /// <summary>
        /// When a discount is applied to the order, the tax calculation has already been applied, and is reflected in the tax.
        /// Therefore, a discount applying to the order  will occur after tax. 
        /// For instance, if the cart subtotal is $100, and $15 is the tax subtotal, a cart-wide discount of 10% will yield a total of $105 ($100 subtotal – $10 discount + $15 tax on the original $100).
        /// </summary>
		public decimal DiscountAmount { get; set; }
      
        #region ITaxDetailSupport Members

        public ICollection<TaxDetail> TaxDetails { get; set; }

        #endregion

        #region ISupportSecurityScopes Members
        public IEnumerable<string> Scopes { get; set; }
        #endregion


        /// <summary>
        /// Grand order total
        /// </summary>
        public virtual decimal Total { get; set; }


        public virtual decimal SubTotal { get; set; }

        public virtual decimal SubTotalWithTax { get; set; }
        public virtual decimal SubTotalDiscount { get; set; }

        public virtual decimal SubTotalDiscountWithTax { get; set; }

        public virtual decimal SubTotalTaxTotal { get; set; }

        public virtual decimal ShippingTotal { get; set; }

        public virtual decimal ShippingTotalWithTax { get; set; }

        public virtual decimal ShippingSubTotal { get; set; }

        public virtual decimal ShippingSubTotalWithTax { get; set; }

        public virtual decimal ShippingDiscountTotal { get; set; }

        public virtual decimal ShippingDiscountTotalWithTax { get; set; }

        public virtual decimal ShippingTaxTotal { get; set; }

        public virtual decimal PaymentTotal { get; set; }

        public virtual decimal PaymentTotalWithTax { get; set; }


        public virtual decimal PaymentSubTotal { get; set; }

        public virtual decimal PaymentSubTotalWithTax { get; set; }

        public virtual decimal PaymentDiscountTotal { get; set; }

        public virtual decimal PaymentDiscountTotalWithTax { get; set; }

        public virtual decimal PaymentTaxTotal { get; set; }

        public virtual decimal DiscountTotal { get; set; }

        public virtual decimal DiscountTotalWithTax { get; set; }


        //Any extra Fee 
        public decimal Fee { get; set; }

        public virtual decimal FeeWithTax { get; set; }

        public virtual decimal FeeTotal { get; set; }

        public virtual decimal FeeTotalWithTax { get; set; }
        #region ITaxable Members

        /// <summary>
        /// Tax category or type
        /// </summary>
        public string TaxType { get; set; }

        public virtual decimal TaxTotal { get; set; }

        public decimal TaxPercentRate { get; set; }

        #endregion

        #region ILanguageSupport Members

        public string LanguageCode { get; set; }

        #endregion     
    }
}
