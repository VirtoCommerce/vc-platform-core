using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.SubscriptionModule.Core.Model.Search
{
    public class SubscriptionSearchResult : GenericSearchResult<Subscription>
    {
        public IList<Subscription> Subscriptions => Results;
    }
}
