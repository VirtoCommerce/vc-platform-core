using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.SubscriptionModule.Core.Model
{
    public class PaymentPlan : AuditableEntity, ICloneable
    {
        public PaymentPlan()
        {
            Interval = PaymentInterval.Months;
        }
        /// <summary>
        /// (days, months, years) - billing interval
        /// </summary>
        public PaymentInterval Interval { get; set; }
        /// <summary>
        /// - to set more customized intervals (every 5 month)
        /// </summary>
        public int IntervalCount { get; set; }
        /// <summary>
        ///  subscription trial period in days 
        /// </summary>
        public int TrialPeriodDays { get; set; }

        #region ICloneable members

        public virtual object Clone()
        {
            return MemberwiseClone() as PaymentPlan;
        }

        #endregion
    }
}
