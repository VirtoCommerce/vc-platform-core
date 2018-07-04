using System;

namespace VirtoCommerce.CartModule.Data.Model
{
    [Flags]
    public enum CartResponseGroup
    {
        Default = 0,
        WithPayments = 1,
        WithLineItems = 2,
        WithShipments = 4,
        Full = 7
    }
}
