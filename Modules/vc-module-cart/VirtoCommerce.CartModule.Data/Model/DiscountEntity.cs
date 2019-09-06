using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Data.Model
{
    public class DiscountEntity : Entity
    {
        [StringLength(64)]
        public string PromotionId { get; set; }

        [StringLength(1024)]
        public string PromotionDescription { get; set; }

        [StringLength(64)]
        public string CouponCode { get; set; }

        [Required]
        [StringLength(3)]
        public string Currency { get; set; }

        [Column(TypeName = "Money")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "Money")]
        public decimal DiscountAmountWithTax { get; set; }

        // navigation properties
        public string ShoppingCartId { get; set; }
        public virtual ShoppingCartEntity ShoppingCart { get; set; }

        public string ShipmentId { get; set; }
        public virtual ShipmentEntity Shipment { get; set; }

        public string LineItemId { get; set; }
        public virtual LineItemEntity LineItem { get; set; }

        public string PaymentId { get; set; }
        public virtual PaymentEntity Payment { get; set; }

        public virtual Discount ToModel(Discount model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.Id = Id;

            model.PromotionId = PromotionId;
            model.Description = PromotionDescription;
            model.Coupon = CouponCode;
            model.Currency = Currency;
            model.DiscountAmount = DiscountAmount;
            model.DiscountAmountWithTax = DiscountAmountWithTax;

            return model;
        }

        public virtual DiscountEntity FromModel(Discount model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            Id = model.Id;

            PromotionId = model.PromotionId;
            PromotionDescription = model.Description;
            CouponCode = model.Coupon;
            Currency = model.Currency;
            DiscountAmount = model.DiscountAmount;
            DiscountAmountWithTax = model.DiscountAmountWithTax;

            return this;
        }

        public virtual void Patch(DiscountEntity target)
        {
            target.PromotionId = PromotionId;
            target.PromotionDescription = PromotionDescription;
            target.CouponCode = CouponCode;
            target.Currency = Currency;
            target.DiscountAmount = DiscountAmount;
            target.DiscountAmountWithTax = DiscountAmountWithTax;
        }
    }
}
