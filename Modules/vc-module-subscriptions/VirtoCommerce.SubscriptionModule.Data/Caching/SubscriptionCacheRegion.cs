using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.SubscriptionModule.Core.Model;

namespace VirtoCommerce.SubscriptionModule.Data.Caching
{
    public class SubscriptionCacheRegion : CancellableCacheRegion<SubscriptionCacheRegion>
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _subscriptionRegionTokenLookup = new ConcurrentDictionary<string, CancellationTokenSource>();

        public static IChangeToken CreateChangeToken(Subscription subscription)
        {
            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }
            var cancellationTokenSource = _subscriptionRegionTokenLookup.GetOrAdd(subscription.Id, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpireSubscription(Subscription subscription)
        {
            if (_subscriptionRegionTokenLookup.TryRemove(subscription.Id, out var token))
            {
                token.Cancel();
            }
        }
    }
}
