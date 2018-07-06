using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.Platform.Core.Caching;

namespace VirtoCommerce.CartModule.Data.Caching
{
    public class CartCacheRegion : CancellableCacheRegion<CartCacheRegion>
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _cartRegionTokenLookup = new ConcurrentDictionary<string, CancellationTokenSource>();

        public static IChangeToken CreateChangeToken(ShoppingCart cart)
        {
            if (cart == null)
            {
                throw new ArgumentNullException(nameof(cart));
            }
            var cancellationTokenSource = _cartRegionTokenLookup.GetOrAdd(cart.Id, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpireInventory(ShoppingCart cart)
        {
            if (_cartRegionTokenLookup.TryRemove(cart.Id, out CancellationTokenSource token))
            {
                token.Cancel();
            }
        }
    }
}
