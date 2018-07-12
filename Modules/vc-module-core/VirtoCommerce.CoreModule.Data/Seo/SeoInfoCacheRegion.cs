using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Platform.Core.Caching;

namespace VirtoCommerce.CoreModule.Data.Caching
{
    public class SeoInfoCacheRegion : CancellableCacheRegion<SeoInfoCacheRegion>
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _seoRegionTokenLookup = new ConcurrentDictionary<string, CancellationTokenSource>();

        public static IChangeToken CreateChangeToken(string seoInfoId)
        {
            if (seoInfoId == null)
            {
                throw new ArgumentNullException(nameof(seoInfoId));
            }
            var cancellationTokenSource = _seoRegionTokenLookup.GetOrAdd(seoInfoId, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpireSeoInfos(IEnumerable<string> seoInfoIds)
        {
            if (seoInfoIds == null)
            {
                throw new ArgumentNullException(nameof(seoInfoIds));
            }
            foreach (var seoInfoId in seoInfoIds)
            {
                ExpireSeoInfo(seoInfoId);
            }
        }

        public static void ExpireSeoInfo(string seoInfoId)
        {
            if (_seoRegionTokenLookup.TryRemove(seoInfoId, out CancellationTokenSource token))
            {
                token.Cancel();
            }
        }
    }
}
