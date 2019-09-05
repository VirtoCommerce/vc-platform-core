using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SubscriptionModule.Core.Model;

namespace VirtoCommerce.SubscriptionModule.Data.Model
{
    public class PaymentPlanEntity : AuditableEntity
    {
        [StringLength(64)]
        public string Interval { get; set; }

        public int IntervalCount { get; set; }

        public int TrialPeriodDays { get; set; }

        [StringLength(128)]
        public string ProductId { get; set; }

        public virtual PaymentPlan ToModel(PaymentPlan paymentPlan)
        {
            if (paymentPlan == null)
            {
                throw new ArgumentNullException(nameof(paymentPlan));
            }
            paymentPlan.Id = Id;
            paymentPlan.CreatedBy = CreatedBy;
            paymentPlan.CreatedDate = CreatedDate;
            paymentPlan.ModifiedBy = ModifiedBy;
            paymentPlan.ModifiedDate = ModifiedDate;

            paymentPlan.IntervalCount = IntervalCount;
            paymentPlan.TrialPeriodDays = TrialPeriodDays;
            paymentPlan.Interval = EnumUtility.SafeParse(Interval, PaymentInterval.Months);

            return paymentPlan;
        }

        public virtual PaymentPlanEntity FromModel(PaymentPlan paymentPlan, PrimaryKeyResolvingMap pkMap)
        {
            if (paymentPlan == null)
            {
                throw new ArgumentNullException(nameof(paymentPlan));
            }

            pkMap.AddPair(paymentPlan, this);

            Id = paymentPlan.Id;
            CreatedBy = paymentPlan.CreatedBy;
            CreatedDate = paymentPlan.CreatedDate;
            ModifiedBy = paymentPlan.ModifiedBy;
            ModifiedDate = paymentPlan.ModifiedDate;

            IntervalCount = paymentPlan.IntervalCount;
            TrialPeriodDays = paymentPlan.TrialPeriodDays;
            Interval = paymentPlan.Interval.ToString();

            return this;
        }

        public virtual void Patch(PaymentPlanEntity target)
        {
            target.Interval = Interval;
            target.IntervalCount = IntervalCount;
            target.ProductId = ProductId;
            target.TrialPeriodDays = TrialPeriodDays;
        }
    }
}
