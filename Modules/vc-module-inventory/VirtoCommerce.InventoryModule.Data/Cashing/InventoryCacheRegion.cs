using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.InventoryModule.Data.Cashing
{
    public class InventoryCacheRegion : CancellableCacheRegion<InventoryCacheRegion>
    {
        private static readonly ConcurrentDictionary<InventoryInfo, CancellationTokenSource> _inventoryRegionTokenLookup = new ConcurrentDictionary<InventoryInfo, CancellationTokenSource>();

        public static IChangeToken CreateChangeToken(InventoryInfo inventory)
        {
            if (inventory == null)
            {
                throw new ArgumentNullException(nameof(inventory));
            }
            var cancellationTokenSource = _inventoryRegionTokenLookup.GetOrAdd(inventory, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpireInventory(InventoryInfo inventory)
        {
            if (_inventoryRegionTokenLookup.TryRemove(inventory, out CancellationTokenSource token))
            {
                token.Cancel();
            }
        }
    }
}
