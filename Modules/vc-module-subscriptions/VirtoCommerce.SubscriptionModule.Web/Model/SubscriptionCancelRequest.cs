using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtoCommerce.SubscriptionModule.Web.Model
{
    public class SubscriptionCancelRequest
    {
        public string SubscriptionId { get; set; }
        public string CancelReason { get; set; }
    }
}