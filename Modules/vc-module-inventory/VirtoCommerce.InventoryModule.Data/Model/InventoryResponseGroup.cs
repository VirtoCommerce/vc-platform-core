using System;

namespace VirtoCommerce.InventoryModule.Data.Model
{
    [Flags]
    public enum InventoryResponseGroup
    {
        Default = 0,
        WithFulfillmentCenter = 1 << 0,
        Full = Default | WithFulfillmentCenter
    }
}
