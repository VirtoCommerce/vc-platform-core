using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.OrdersModule.Core.Model.Search
{
    public class CustomerOrderSearchResult : GenericSearchResult<CustomerOrder>
    {
        public IList<CustomerOrder> CustomerOrders => Results;
    }
}
