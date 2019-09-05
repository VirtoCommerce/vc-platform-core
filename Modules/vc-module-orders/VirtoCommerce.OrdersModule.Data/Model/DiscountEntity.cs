using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.OrdersModule.Data.Model
{
    public class DiscountEntity : Entity
    {
        [StringLength(64)]
        public string PromotionId { get; set; }
        [StringLength(1024)]
        public string PromotionDescription { get; set; }
        [Required]
        [StringLength(3)]
        public string Currency { get; set; }
        [Column(TypeName = "Money")]
        public decimal DiscountAmount { get; set; }
        [Column(TypeName = "Money")]
        public decimal DiscountAmountWithTax { get; set; }
        [StringLength(64)]
        public string CouponCode { get; set; }
        [StringLength(1024)]
        public string CouponInvalidDescription { get; set; }

        #region Navigation Properties

        public virtual CustomerOrderEntity CustomerOrder { get; set; }
        public string CustomerOrderId { get; set; }

        public virtual ShipmentEntity Shipment { get; set; }
        public string ShipmentId { get; set; }

        public virtual LineItemEntity LineItem { get; set; }
        public string LineItemId { get; set; }

        public virtual PaymentInEntity PaymentIn { get; set; }
        public string PaymentInId { get; set; }

        #endregion

        public virtual Discount ToModel(Discount discount)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            discount.Id = Id;
            discount.Currency = Currency;
            discount.DiscountAmount = DiscountAmount;
            discount.DiscountAmountWithTax = DiscountAmountWithTax;
            discount.Description = PromotionDescription;
            discount.PromotionId = PromotionId;
            discount.Coupon = CouponCode;

            return discount;
        }

        public virtual DiscountEntity FromModel(Discount discount)
        {
            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            Id = discount.Id;
            Currency = discount.Currency;
            DiscountAmount = discount.DiscountAmount;
            DiscountAmountWithTax = discount.DiscountAmountWithTax;
            PromotionDescription = discount.Description;
            PromotionId = discount.PromotionId;
            CouponCode = discount.Coupon;

            return this;
        }

        public virtual void Patch(DiscountEntity target)
        {
            target.CouponCode = CouponCode;
            target.Currency = Currency;
            target.DiscountAmount = DiscountAmount;
            target.DiscountAmountWithTax = DiscountAmountWithTax;
            target.PromotionDescription = PromotionDescription;
            target.PromotionId = PromotionId;
        }
    }
}
