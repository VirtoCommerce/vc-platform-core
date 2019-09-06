using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SubscriptionModule.Core.Model;

namespace VirtoCommerce.SubscriptionModule.Data.Model
{
    public class SubscriptionEntity : AuditableEntity, IHasOuterId
    {

        [Required]
        [StringLength(64)]
        public string StoreId { get; set; }

        [Required]
        [StringLength(64)]
        public string CustomerId { get; set; }
        [StringLength(255)]
        public string CustomerName { get; set; }

        [Required]
        [StringLength(64)]
        public string Number { get; set; }

        [Column(TypeName = "Money")]
        public decimal Balance { get; set; }

        [StringLength(64)]
        public string Interval { get; set; }

        public int IntervalCount { get; set; }

        public int TrialPeriodDays { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public DateTime? TrialSart { get; set; }
        public DateTime? TrialEnd { get; set; }

        public DateTime? CurrentPeriodStart { get; set; }

        public DateTime? CurrentPeriodEnd { get; set; }

        [StringLength(64)]
        public string Status { get; set; }

        public bool IsCancelled { get; set; }
        public DateTime? CancelledDate { get; set; }
        [StringLength(2048)]
        public string CancelReason { get; set; }

        public string CustomerOrderPrototypeId { get; set; }

        public string Comment { get; set; }

        [StringLength(128)]
        public string OuterId { get; set; }

        public virtual Subscription ToModel(Subscription subscription)
        {
            if (subscription == null)
            {
                throw new NullReferenceException(nameof(subscription));
            }

            subscription.Id = Id;
            subscription.CreatedBy = CreatedBy;
            subscription.CreatedDate = CreatedDate;
            subscription.ModifiedBy = ModifiedBy;
            subscription.ModifiedDate = ModifiedDate;
            subscription.OuterId = OuterId;

            subscription.Balance = Balance;
            subscription.CancelledDate = CancelledDate;
            subscription.CancelReason = CancelReason;
            subscription.CurrentPeriodEnd = CurrentPeriodEnd;
            subscription.CurrentPeriodStart = CurrentPeriodStart;
            subscription.CustomerId = CustomerId;
            subscription.CustomerName = CustomerName;
            subscription.CustomerOrderPrototypeId = CustomerOrderPrototypeId;
            subscription.EndDate = EndDate;
            subscription.IntervalCount = IntervalCount;
            subscription.IsCancelled = IsCancelled;
            subscription.Number = Number;
            subscription.StartDate = StartDate;
            subscription.StoreId = StoreId;
            subscription.TrialEnd = TrialEnd;
            subscription.TrialPeriodDays = TrialPeriodDays;
            subscription.TrialSart = TrialSart;
            subscription.Comment = Comment;

            subscription.SubscriptionStatus = EnumUtility.SafeParse(Status, SubscriptionStatus.Active);
            subscription.Interval = EnumUtility.SafeParse(Interval, PaymentInterval.Months);
            return subscription;
        }

        public virtual SubscriptionEntity FromModel(Subscription subscription, PrimaryKeyResolvingMap pkMap)
        {
            if (subscription == null)
            {
                throw new NullReferenceException(nameof(subscription));
            }

            pkMap.AddPair(subscription, this);

            Id = subscription.Id;
            CreatedBy = subscription.CreatedBy;
            CreatedDate = subscription.CreatedDate;
            ModifiedBy = subscription.ModifiedBy;
            ModifiedDate = subscription.ModifiedDate;
            OuterId = subscription.OuterId;

            Balance = subscription.Balance;
            CancelledDate = subscription.CancelledDate;
            CancelReason = subscription.CancelReason;
            CurrentPeriodEnd = subscription.CurrentPeriodEnd;
            CurrentPeriodStart = subscription.CurrentPeriodStart;
            CustomerId = subscription.CustomerId;
            CustomerName = subscription.CustomerName;
            CustomerOrderPrototypeId = subscription.CustomerOrderPrototypeId;
            EndDate = subscription.EndDate;
            IntervalCount = subscription.IntervalCount;
            IsCancelled = subscription.IsCancelled;
            Number = subscription.Number;
            StartDate = subscription.StartDate;
            StoreId = subscription.StoreId;
            TrialEnd = subscription.TrialEnd;
            TrialPeriodDays = subscription.TrialPeriodDays;
            TrialSart = subscription.TrialSart;
            Comment = subscription.Comment;


            if (subscription.CustomerOrderPrototype != null)
            {
                CustomerOrderPrototypeId = subscription.CustomerOrderPrototype.Id;
            }

            Status = subscription.SubscriptionStatus.ToString();
            Interval = subscription.Interval.ToString();
            return this;
        }

        public virtual void Patch(SubscriptionEntity target)
        {
            if (target == null)
            {
                throw new NullReferenceException(nameof(target));
            }

            target.CustomerOrderPrototypeId = CustomerOrderPrototypeId;
            target.CustomerId = CustomerId;
            target.CustomerName = CustomerName;
            target.StoreId = StoreId;
            target.Number = Number;
            target.IsCancelled = IsCancelled;
            target.CancelledDate = CancelledDate;
            target.CancelReason = CancelReason;
            target.Status = Status;
            target.Interval = Interval;
            target.IntervalCount = IntervalCount;
            target.TrialPeriodDays = TrialPeriodDays;
            target.Balance = Balance;
            target.StartDate = StartDate;
            target.EndDate = EndDate;
            target.TrialSart = TrialSart;
            target.TrialEnd = TrialEnd;
            target.CurrentPeriodStart = CurrentPeriodStart;
            target.CurrentPeriodEnd = CurrentPeriodEnd;
            target.OuterId = OuterId;
            target.Comment = Comment;
        }
    }
}
