using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Platform.Core.Caching;

namespace VirtoCommerce.PricingModule.Data.Caching
{
    public class PricesCacheRegion : CancellableCacheRegion<PricesCacheRegion>
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _priceRegionTokenLookup = new ConcurrentDictionary<string, CancellationTokenSource>();

        public static IChangeToken CreateChangeToken(string priceId)
        {
            if (string.IsNullOrEmpty(priceId))
            {
                throw new ArgumentNullException(nameof(priceId));
            }
            var cancellationTokenSource = _priceRegionTokenLookup.GetOrAdd(priceId, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpirePrice(string priceId)
        {
            if (_priceRegionTokenLookup.TryRemove(priceId, out var token))
            {
                token.Cancel();
            }
        }
    }
}
