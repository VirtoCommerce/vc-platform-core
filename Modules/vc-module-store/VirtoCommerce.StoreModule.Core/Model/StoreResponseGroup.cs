using System;

namespace VirtoCommerce.StoreModule.Core.Model
{
    [Flags]
    public enum StoreResponseGroup
    {
        None = 0,
        /// <summary>
        /// 
        /// </summary>
        StoreInfo = 1,
        /// <summary>
        /// 
        /// </summary>
        StoreFulfillmentCenters = 1 << 1,
        WithFulfillmentCenters = StoreFulfillmentCenters,
        StoreSeoInfos = 1 << 2,
        WithSeoInfos = StoreSeoInfos,
        StoreDynamicPropertyObjectValues = 1 << 3,
        WithDynamicPropertyObjectValues = StoreDynamicPropertyObjectValues,

        Full = StoreFulfillmentCenters | StoreSeoInfos | StoreDynamicPropertyObjectValues
    }
}
