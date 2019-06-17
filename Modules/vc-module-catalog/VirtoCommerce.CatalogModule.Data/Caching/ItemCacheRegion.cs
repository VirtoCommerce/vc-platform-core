using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Caching;

namespace VirtoCommerce.CatalogModule.Data.Caching
{
    public class ItemCacheRegion : CancellableCacheRegion<ItemCacheRegion>
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _entityRegionTokenLookup = new ConcurrentDictionary<string, CancellationTokenSource>();

        public static IChangeToken CreateChangeToken(CatalogProduct entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            var cancellationTokenSource = _entityRegionTokenLookup.GetOrAdd(entity.Id, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpireEntity(CatalogProduct entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            if (_entityRegionTokenLookup.TryRemove(entity.Id, out var token))
            {
                token.Cancel();
            }
            //need to also evict from cache a main product if given product is variation
            if (entity.MainProductId != null)
            {
                if (_entityRegionTokenLookup.TryRemove(entity.MainProductId, out var token2))
                {
                    token2.Cancel();
                }
            }
        }
    }
}
