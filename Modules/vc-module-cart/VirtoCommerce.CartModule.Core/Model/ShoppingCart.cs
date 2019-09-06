using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Tax;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.CartModule.Core.Model
{
    public class ShoppingCart : AuditableEntity, IHasTaxDetalization, IHasDynamicProperties, ITaxable, IHasDiscounts, ICloneable
    {
        public string Name { get; set; }
        public string StoreId { get; set; }
        public string ChannelId { get; set; }
        public bool IsAnonymous { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string OrganizationId { get; set; }
        public string Currency { get; set; }

        public string LanguageCode { get; set; }
        public bool? TaxIncluded { get; set; }
        public bool? IsRecuring { get; set; }
        public string Comment { get; set; }

        public string Status { get; set; }

        public string WeightUnit { get; set; }
        public decimal? Weight { get; set; }

        /// <summary>
        /// Represent any line item validation type (noPriceValidate, noQuantityValidate etc) this value can be used in storefront 
        /// to select appropriate validation strategy
        /// </summary>
        public string ValidationType { get; set; }

        /// <summary>
        /// Represents a cart object type (Wishlist, Cart etc) for fitering purposes
        /// </summary>
        public string Type { get; set; }

        public decimal? VolumetricWeight { get; set; }

        //Grand  cart total
        public virtual decimal Total { get; set; }

        public virtual decimal SubTotal { get; set; }

        public virtual decimal SubTotalWithTax { get; set; }

        public virtual decimal SubTotalDiscount { get; set; }

        public virtual decimal SubTotalDiscountWithTax { get; set; }


        public virtual decimal ShippingTotal { get; set; }

        public virtual decimal ShippingTotalWithTax { get; set; }

        public virtual decimal ShippingSubTotal { get; set; }

        public virtual decimal ShippingSubTotalWithTax { get; set; }

        public virtual decimal ShippingDiscountTotal { get; set; }

        public virtual decimal ShippingDiscountTotalWithTax { get; set; }


        public virtual decimal PaymentTotal { get; set; }

        public virtual decimal PaymentTotalWithTax { get; set; }
        public virtual decimal PaymentSubTotal { get; set; }

        public virtual decimal PaymentSubTotalWithTax { get; set; }

        public virtual decimal PaymentDiscountTotal { get; set; }

        public virtual decimal PaymentDiscountTotalWithTax { get; set; }
        public virtual decimal HandlingTotal { get; set; }
        public virtual decimal HandlingTotalWithTax { get; set; }

        public virtual decimal DiscountAmount { get; set; }
        public virtual decimal DiscountAmountWithTax { get; set; }

        public virtual decimal DiscountTotal { get; set; }
        public virtual decimal DiscountTotalWithTax { get; set; }

        //Any extra Fee 
        public decimal Fee { get; set; }

        public virtual decimal FeeWithTax { get; set; }

        public virtual decimal FeeTotal { get; set; }

        public virtual decimal FeeTotalWithTax { get; set; }

        public ICollection<Address> Addresses { get; set; }
        public ICollection<LineItem> Items { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public ICollection<Shipment> Shipments { get; set; }
        /// <summary>
        /// Entered multiple coupons
        /// </summary>
        private ICollection<string> _coupons;
        public ICollection<string> Coupons
        {
            get
            {
                if (_coupons == null && !string.IsNullOrEmpty(Coupon))
                {
                    _coupons = new List<string>() { Coupon };
                }
                return _coupons;
            }
            set
            {
                _coupons = value;
            }
        }
        public string Coupon { get; set; }

        #region IHasDiscounts
        public ICollection<Discount> Discounts { get; set; }
        #endregion

        #region ITaxable Members

        /// <summary>
        /// Tax category or type
        /// </summary>
        public string TaxType { get; set; }

        public virtual decimal TaxTotal { get; set; }

        public decimal TaxPercentRate { get; set; }

        #endregion

        #region IHaveTaxDetalization Members
        public ICollection<TaxDetail> TaxDetails { get; set; }
        #endregion

        #region IHasDynamicProperties Members
        public string ObjectType => typeof(ShoppingCart).FullName;
        public ICollection<DynamicObjectProperty> DynamicProperties { get; set; }

        #endregion

        public virtual void ReduceDetails(string responseGroup)
        {
            //Reduce details according to response group
            var cartResponseGroup = EnumUtility.SafeParseFlags(responseGroup, CartResponseGroup.Full);

            if (!cartResponseGroup.HasFlag(CartResponseGroup.WithLineItems))
            {
                Items = null;
            }
            if (!cartResponseGroup.HasFlag(CartResponseGroup.WithPayments))
            {
                Payments = null;
            }
            if (!cartResponseGroup.HasFlag(CartResponseGroup.WithShipments))
            {
                Shipments = null;
            }
            if (!cartResponseGroup.HasFlag(CartResponseGroup.WithDynamicProperties))
            {
                DynamicProperties = null;
            }
        }

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as ShoppingCart;

            result.Discounts = Discounts?.Select(x => x.Clone()).OfType<Discount>().ToList();
            result.Addresses = Addresses?.Select(x => x.Clone()).OfType<Address>().ToList();
            result.Items = Items?.Select(x => x.Clone()).OfType<LineItem>().ToList();
            result.Payments = Payments?.Select(x => x.Clone()).OfType<Payment>().ToList();
            result.Shipments = Shipments?.Select(x => x.Clone()).OfType<Shipment>().ToList();
            result.TaxDetails = TaxDetails?.Select(x => x.Clone()).OfType<TaxDetail>().ToList();
            result.DynamicProperties = DynamicProperties?.Select(x => x.Clone()).OfType<DynamicObjectProperty>().ToList();

            return result;
        }

        #endregion
    }
}
