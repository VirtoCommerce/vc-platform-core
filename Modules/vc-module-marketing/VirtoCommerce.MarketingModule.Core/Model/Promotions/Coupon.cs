using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    public class Coupon : AuditableEntity, ICloneable
    {
        /// <summary>
        /// Restriction of total coupon usages
        /// 0 infinitive
        /// </summary>
        public int MaxUsesNumber { get; set; }

        /// <summary>
        /// Maximum number of uses per registered user
        /// 0 infinitive
        /// </summary>
        public int MaxUsesPerUser { get; set; }

        public DateTime? ExpirationDate { get; set; }
        /// <summary>
        /// coupon code
        /// </summary>
        public string Code { get; set; }
        public string PromotionId { get; set; }
        /// <summary>
        /// Total number of uses 
        /// </summary>
        public long TotalUsesCount { get; set; }
        public string OuterId { get; set; }

        #region ICloneable members

        public virtual object Clone()
        {
            return MemberwiseClone() as Coupon;
        }

        #endregion
    }
}
