using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    public class PromotionEvaluationContext : EvaluationContextBase
    {
        public string[] RefusedGiftIds { get; set; }

        public string StoreId { get; set; }

        public string Currency { get; set; }

        /// <summary>
        /// Customer id
        /// </summary>
        public string CustomerId { get; set; }

        public bool IsRegisteredUser { get; set; }

        /// <summary>
        /// Has user made any orders
        /// </summary>
        public bool IsFirstTimeBuyer { get; set; }

        public bool IsEveryone { get; set; } = true;

        //Cart subtotal (incorrect property name cannot change for back compatibility reasons)
        public decimal CartTotal { get; set; }

        /// <summary>
        /// Current shipment method
        /// </summary>
        public string ShipmentMethodCode { get; set; }
        public string ShipmentMethodOption { get; set; }
        public decimal ShipmentMethodPrice { get; set; }
        public string[] AvailableShipmentMethodCodes { get; set; }

        /// <summary>
        /// Current payment method
        /// </summary>
        public string PaymentMethodCode { get; set; }
        public decimal PaymentMethodPrice { get; set; }
        public string[] AvailablePaymentMethodCodes { get; set; }


        /// <summary>
        /// Entered coupon
        /// </summary>
        public string Coupon { get; set; }
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
        /// <summary>
        /// List of product promo in cart
        /// </summary>
        public ICollection<ProductPromoEntry> CartPromoEntries { get; set; } = new List<ProductPromoEntry>();
        /// <summary>
        /// List of products for promo evaluation
        /// </summary>
        public ICollection<ProductPromoEntry> PromoEntries { get; set; } = new List<ProductPromoEntry>();
        /// <summary>
        /// Single catalog product promo entry 
        /// </summary>
        public ProductPromoEntry PromoEntry { get; set; }




    }

}
