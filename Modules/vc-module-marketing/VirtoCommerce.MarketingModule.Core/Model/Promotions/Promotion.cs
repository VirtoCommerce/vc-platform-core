using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Core.Model.Promotions
{
    public class Promotion : AuditableEntity, ICloneable
    {
        public Promotion()
        {
            IsActive = true;
        }

        public string Store { get; set; }

        public IList<string> StoreIds { get; set; }

        /// <summary>
        /// Promotion name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Required for UI. TODO: remove later
        /// </summary>
        public string Type { get; set; }

        public bool IsActive { get; set; }

        /// <summary>
        /// Represents a promotion priority, for combination policies when it is necessary to select a promotion with a higher priority
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// If a promotion with this setting is applied, no other promotions can be applied to the order.
        /// </summary>
        public bool IsExclusive { get; set; }

        public bool HasCoupons { get; set; }

        public string Description { get; set; }
        /// <summary>
        /// Maximum redemptions for this promotion
        /// </summary>
        public int MaxUsageCount { get; set; }

        /// <summary>
        /// Maximum redemptions on a single order
        /// </summary>
        public int MaxUsageOnOrder { get; set; }

        /// <summary>
        /// Maximum redemptions by a single customer
        /// </summary>
        public int MaxPersonalUsageCount { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string OuterId { get; set; }

        public virtual void ReduceDetails(string responseGroup)
        {
            //Nothing todo
        }
        public virtual Task<PromotionReward[]> EvaluatePromotionAsync(IEvaluationContext context)
        {
            return Task.FromResult(Array.Empty<PromotionReward>());
        }

        #region ICloneable members

        public virtual object Clone()
        {
            return MemberwiseClone() as Promotion;
        }

        #endregion
    }
}
