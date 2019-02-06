using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.SitemapsModule.Core.Models;

namespace VirtoCommerce.SitemapsModule.Data.Caching
{
    public class SitemapCacheRegion : CancellableCacheRegion<SitemapCacheRegion>
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _sitemapRegionTokenLookup = new ConcurrentDictionary<string, CancellationTokenSource>();

        public static IChangeToken CreateChangeToken(string sitemapId)
        {
            if (string.IsNullOrEmpty(sitemapId))
            {
                throw new ArgumentNullException(nameof(sitemapId));
            }
            var cancellationTokenSource = _sitemapRegionTokenLookup.GetOrAdd(sitemapId, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpireSitemap(string sitemapId)
        {
            if (_sitemapRegionTokenLookup.TryRemove(sitemapId, out var token))
            {
                token.Cancel();
            }
        }
    }
}
