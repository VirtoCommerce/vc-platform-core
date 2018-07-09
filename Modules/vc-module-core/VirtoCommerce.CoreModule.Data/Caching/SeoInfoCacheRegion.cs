using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.CoreModule.Core.Model;
using VirtoCommerce.Platform.Core.Caching;

namespace VirtoCommerce.CoreModule.Data.Caching
{
    public class SeoInfoCacheRegion : CancellableCacheRegion<SeoInfoCacheRegion>
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _seoRegionTokenLookup = new ConcurrentDictionary<string, CancellationTokenSource>();

        public static IChangeToken CreateChangeToken(SeoInfo seoInfo)
        {
            if (seoInfo == null)
            {
                throw new ArgumentNullException(nameof(seoInfo));
            }
            var cancellationTokenSource = _seoRegionTokenLookup.GetOrAdd(seoInfo.Id, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpireSeoInfo(SeoInfo seoInfo)
        {
            if (_seoRegionTokenLookup.TryRemove(seoInfo.Id, out CancellationTokenSource token))
            {
                token.Cancel();
            }
        }
    }
}
