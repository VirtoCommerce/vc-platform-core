using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.SubscriptionModule.Core.Model
{
    public enum SubscriptionStatus
    {

        Active,
        /// <summary>
        ///  active when the trial period is over
        /// </summary>
        Trialing,
        /// <summary>
        /// When payment to renew the subscription fails
        /// </summary>
        PastDue,
        /// <summary>
        /// the subscription ends up with a status of either canceled 
        /// </summary>
        Cancelled,
        /// <summary>
        /// When subscription have some unpaid orders
        /// </summary>
        Unpaid,
    }
}
