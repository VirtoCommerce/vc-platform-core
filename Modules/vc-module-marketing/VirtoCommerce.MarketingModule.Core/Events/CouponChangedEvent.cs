using System.Collections.Generic;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.MarketingModule.Core.Events
{
    public class CouponChangedEvent : GenericChangedEntryEvent<Coupon>
    {
        public CouponChangedEvent(IEnumerable<GenericChangedEntry<Coupon>> changedEntries) : base(changedEntries)
        {
        }
    }
}
