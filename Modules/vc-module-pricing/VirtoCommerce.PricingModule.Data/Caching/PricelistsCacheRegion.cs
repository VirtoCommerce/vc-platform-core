using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Platform.Core.Caching;

namespace VirtoCommerce.PricingModule.Data.Caching
{
    public class PricelistsCacheRegion : CancellableCacheRegion<PricelistsCacheRegion>
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _pricelistRegionTokenLookup = new ConcurrentDictionary<string, CancellationTokenSource>();

        public static IChangeToken CreateChangeToken(string pricelistId)
        {
            if (string.IsNullOrEmpty(pricelistId))
            {
                throw new ArgumentNullException(nameof(pricelistId));
            }
            var cancellationTokenSource = _pricelistRegionTokenLookup.GetOrAdd(pricelistId, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpirePricelist(string pricelistId)
        {
            if (_pricelistRegionTokenLookup.TryRemove(pricelistId, out var token))
            {
                token.Cancel();
            }
        }
    }
}
