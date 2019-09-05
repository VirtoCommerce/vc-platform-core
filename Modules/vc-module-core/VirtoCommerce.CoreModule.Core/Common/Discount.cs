using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Common
{
    public class Discount : Entity, ICloneable
    {
        public string PromotionId { get; set; }
        public string Currency { get; set; }
        public virtual decimal DiscountAmount { get; set; }
        public virtual decimal DiscountAmountWithTax { get; set; }
        public string Coupon { get; set; }
        public string Description { get; set; }

        #region ICloneable members

        public virtual object Clone()
        {
            return MemberwiseClone() as Discount;
        }

        #endregion
    }
}
