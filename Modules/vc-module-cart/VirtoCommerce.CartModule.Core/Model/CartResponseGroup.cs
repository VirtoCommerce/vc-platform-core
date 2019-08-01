using System;

namespace VirtoCommerce.CartModule.Core.Model
{
    [Flags]
    public enum CartResponseGroup
    {
        Default = 0,
        WithPayments = 1 ,
        WithLineItems = 1 << 1,
        WithShipments = 1 << 2,
        WithDynamicProperties = 1 << 3,
        Full = Default | WithPayments | WithLineItems | WithShipments | WithDynamicProperties 
    }
}
