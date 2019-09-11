using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
	public class PromotionUsage : AuditableEntity, ICloneable
    {
        //The identifier of the object for which it was used
        public string ObjectId { get; set; }
        public string ObjectType { get; set; }

        public string CouponCode { get; set; }
        public string PromotionId { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; }

        public virtual object Clone()
        {
            return MemberwiseClone() as PromotionUsage;
        }
    }
}
