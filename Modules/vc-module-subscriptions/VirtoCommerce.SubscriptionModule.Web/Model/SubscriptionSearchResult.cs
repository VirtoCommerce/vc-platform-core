using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VirtoCommerce.SubscriptionModule.Core.Model;

namespace VirtoCommerce.SubscriptionModule.Web.Model
{
    public class SubscriptionSearchResult
    {
        public SubscriptionSearchResult()
        {
            Subscriptions = new List<Subscription>();
        }
        public int TotalCount { get; set; }

        public List<Subscription> Subscriptions { get; set; }
    }
}