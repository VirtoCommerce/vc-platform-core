using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Platform.Core.Caching;

namespace VirtoCommerce.PricingModule.Data.Caching
{
    public class PricelistAssignmentsCacheRegion : CancellableCacheRegion<PricelistAssignmentsCacheRegion>
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _pricelistAssignmentRegionTokenLookup = new ConcurrentDictionary<string, CancellationTokenSource>();

        public static IChangeToken CreateChangeToken(string pricelistAssignmentId)
        {
            if (string.IsNullOrEmpty(pricelistAssignmentId))
            {
                throw new ArgumentNullException(nameof(pricelistAssignmentId));
            }

            var cancellationTokenSource = _pricelistAssignmentRegionTokenLookup.GetOrAdd(pricelistAssignmentId, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpirePricelistAssignment(string pricelistAssignmentId)
        {
            if (_pricelistAssignmentRegionTokenLookup.TryRemove(pricelistAssignmentId, out var token))
            {
                token.Cancel();
            }
        }
    }
}
