using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.Platform.Core.Caching;

namespace VirtoCommerce.InventoryModule.Data.Caching
{
    public class InventoryCacheRegion : CancellableCacheRegion<InventoryCacheRegion>
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _inventoryRegionTokenLookup = new ConcurrentDictionary<string, CancellationTokenSource>();

        public static IChangeToken CreateChangeToken(InventoryInfo inventory)
        {
            if (inventory == null)
            {
                throw new ArgumentNullException(nameof(inventory));
            }
            var cancellationTokenSource = _inventoryRegionTokenLookup.GetOrAdd(inventory.ProductId, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpireInventory(InventoryInfo inventory)
        {
            if (_inventoryRegionTokenLookup.TryRemove(inventory.ProductId, out CancellationTokenSource token))
            {
                token.Cancel();
            }
        }
    }
}
