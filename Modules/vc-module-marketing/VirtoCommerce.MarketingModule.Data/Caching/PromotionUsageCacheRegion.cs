using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.Platform.Core.Caching;

namespace VirtoCommerce.MarketingModule.Data.Caching
{
    public class PromotionUsageCacheRegion : CancellableCacheRegion<PromotionUsageCacheRegion>
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _promotionUsageRegionTokenLookup = new ConcurrentDictionary<string, CancellationTokenSource>();

        public static IChangeToken CreateChangeToken(PromotionUsage usage)
        {
            if (usage == null)
            {
                throw new ArgumentNullException(nameof(usage));
            }
            var cancellationTokenSource = _promotionUsageRegionTokenLookup.GetOrAdd(usage.PromotionId, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpireUsages(params PromotionUsage[] usages)
        {
            foreach (var usage in usages)
            {
                if (_promotionUsageRegionTokenLookup.TryRemove(usage.PromotionId, out var token))
                {
                    token.Cancel();
                }
            }
        }

    }
}
