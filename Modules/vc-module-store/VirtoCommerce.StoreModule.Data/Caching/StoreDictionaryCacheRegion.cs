using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.StoreModule.Data.Caching
{
    public class StoreDictionaryCacheRegion : CancellableCacheRegion<StoreDictionaryCacheRegion>
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _dirRegionTokenLookup = new ConcurrentDictionary<string, CancellationTokenSource>();

        public static IChangeToken CreateChangeToken(Store store)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }
            var cancellationTokenSource = _dirRegionTokenLookup.GetOrAdd(store.Id, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpireInventory(Store store)
        {
            if (_dirRegionTokenLookup.TryRemove(store.Id, out CancellationTokenSource token))
            {
                token.Cancel();
            }
        }
    }
}
