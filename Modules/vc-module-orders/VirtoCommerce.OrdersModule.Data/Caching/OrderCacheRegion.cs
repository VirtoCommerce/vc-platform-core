using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.Platform.Core.Caching;

namespace VirtoCommerce.OrdersModule.Data.Caching
{
    public class OrderCacheRegion : CancellableCacheRegion<OrderCacheRegion>
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _orderRegionTokenLookup = new ConcurrentDictionary<string, CancellationTokenSource>();

        public static IChangeToken CreateChangeToken(CustomerOrder order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }
            var cancellationTokenSource = _orderRegionTokenLookup.GetOrAdd(order.Id, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpireOrder(CustomerOrder order)
        {
            if (_orderRegionTokenLookup.TryRemove(order.Id, out CancellationTokenSource token))
            {
                token.Cancel();
            }
        }
    }
}
