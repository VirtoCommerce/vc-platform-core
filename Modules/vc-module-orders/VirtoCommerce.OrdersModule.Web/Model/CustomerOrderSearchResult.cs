using System.Collections.Generic;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.OrdersModule.Web.Model
{
    public class CustomerOrderSearchResult
    {
        public CustomerOrderSearchResult()
        {
            CustomerOrders = new List<CustomerOrder>();
        }

        public int TotalCount { get; set; }

        public List<CustomerOrder> CustomerOrders { get; set; }

    }
}
