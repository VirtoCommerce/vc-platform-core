using System;
using System.Collections.Generic;
using System.Text;

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
