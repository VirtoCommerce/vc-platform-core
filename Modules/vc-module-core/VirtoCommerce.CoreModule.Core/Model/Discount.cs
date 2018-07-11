using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Model
{
    public class Discount : Entity
    {
        public string PromotionId { get; set; }
        public string Currency { get; set; }
        public virtual decimal DiscountAmount { get; set; }
        public virtual decimal DiscountAmountWithTax { get; set; }
        public string Coupon { get; set; }
        public string Description { get; set; }
    }
}
