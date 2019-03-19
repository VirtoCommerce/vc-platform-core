using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Platform.Core.Caching;

namespace VirtoCommerce.ContentModule.Data.Model
{
    public class ContentCacheRegion : CancellableCacheRegion<ContentCacheRegion>
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _dirRegionTokenLookup = new ConcurrentDictionary<string, CancellationTokenSource>();

        public static IChangeToken CreateChangeToken(string contentStore)
        {
            if (contentStore == null)
            {
                throw new ArgumentNullException(nameof(contentStore));
            }
            var cancellationTokenSource = _dirRegionTokenLookup.GetOrAdd(contentStore, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpireContent(string contentStore)
        {
            if (_dirRegionTokenLookup.TryRemove(contentStore, out var token))
            {
                token.Cancel();
            }
        }
    }
}
