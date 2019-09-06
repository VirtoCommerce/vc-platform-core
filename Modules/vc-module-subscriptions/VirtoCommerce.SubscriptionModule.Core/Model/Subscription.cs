using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.SubscriptionModule.Core.Model
{
    public class Subscription : AuditableEntity, IHasChangesHistory, ISupportCancellation, IHasOuterId, ICloneable
    {
        public string StoreId { get; set; }

        public string CustomerId { get; set; }

        public string CustomerName { get; set; }
        /// <summary>
        /// Subscription actual balance
        /// </summary>
        public decimal Balance { get; set; }
        /// <summary>
        /// Subscription number
        /// </summary>
        public string Number { get; set; }
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

        public SubscriptionStatus SubscriptionStatus { get; set; }

        public string CustomerOrderPrototypeId { get; set; }
        /// <summary>
        /// Order prototype for future orders. Changing this prototype can affect for future orders of this subscription
        /// </summary>
        public CustomerOrder CustomerOrderPrototype { get; set; }

        /// <summary>
        /// List of all orders ids created on the basis of the subscription
        /// </summary>
        public IEnumerable<string> CustomerOrdersIds { get; set; }
        /// <summary>
        /// List of all orders  created on the basis of the subscription
        /// </summary>
        public ICollection<CustomerOrder> CustomerOrders { get; set; }

        /// <summary>
        /// Date the most recent update to this subscription started.
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// The date the subscription ended
        /// </summary>
        public DateTime? EndDate { get; set; }

        //If the subscription has a trial, the beginning of that trial.
        public DateTime? TrialSart { get; set; }
        // If the subscription has a trial, the end of that trial.
        public DateTime? TrialEnd { get; set; }

        //Start of the current period that the subscription has been ordered for
        public DateTime? CurrentPeriodStart { get; set; }

        //End of the current period that the subscription has been ordered for. At the end of this period, a new order will be created.
        public DateTime? CurrentPeriodEnd { get; set; }

        /// <summary>
        /// External Subscrption entity system Id
        /// </summary>
        public string OuterId { get; set; }

        /// <summary>
        /// The subscription comment
        /// </summary>
        public string Comment { get; set; }

        #region IHasChangesHistory Members
        public ICollection<OperationLog> OperationsLog { get; set; }
        #endregion

        #region ISupportCancelation Members
        public bool IsCancelled { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string CancelReason { get; set; }
        #endregion

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as Subscription;

            result.CustomerOrderPrototype = CustomerOrderPrototype?.Clone() as CustomerOrder;
            result.CustomerOrders = CustomerOrders?.Select(x => x.Clone()).OfType<CustomerOrder>().ToList();
            result.OperationsLog = OperationsLog?.Select(x => x.Clone()).OfType<OperationLog>().ToList();

            return result;
        }

        #endregion
    }
}
