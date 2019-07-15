using System;

namespace VirtoCommerce.OrdersModule.Core.Model
{
    [Flags]
    public enum CustomerOrderResponseGroup
    {
        Default = 0,
        WithItems = 1,
        WithInPayments = 1 << 1,
        WithShipments = 1 << 2,
        WithAddresses = 1 << 3,
        WithDiscounts = 1 << 4,
        WithPrices = 1 << 5,
        WithDynamicProperties = 1 << 6,
        Full = WithItems | WithInPayments | WithShipments | WithAddresses | WithDiscounts | WithPrices | WithDynamicProperties
    }
}
