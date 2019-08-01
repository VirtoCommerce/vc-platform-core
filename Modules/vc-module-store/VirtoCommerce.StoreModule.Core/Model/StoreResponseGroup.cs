using System;

namespace VirtoCommerce.StoreModule.Core.Model
{
    [Flags]
    public enum StoreResponseGroup
    {
        None = 0,
        StoreInfo = 1,
        StoreFulfillmentCenters = 1 << 1,
        WithFulfillmentCenters = StoreFulfillmentCenters,
        StoreSeoInfos = 1 << 2,
        WithSeoInfos = StoreSeoInfos,
        DynamicProperties = 1 << 3,
        WithDynamicProperties = DynamicProperties,

        Full = StoreFulfillmentCenters | StoreSeoInfos | DynamicProperties
    }
}
