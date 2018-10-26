using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.SubscriptionModule.Core.Model
{
    [Flags]
    public enum SubscriptionResponseGroup
    {
        Default = 1,
        WithChangeLog = 1 << 1,
        WithOrderPrototype = 1 << 2,
        WithRelatedOrders = 1 << 3,
        Full = Default | WithOrderPrototype | WithRelatedOrders | WithChangeLog
    }
}
