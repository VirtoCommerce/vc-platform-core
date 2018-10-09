using System.Collections.Generic;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.OrdersModule2.Web.Model
{
    public class CustomerOrder2 : CustomerOrder
    {
        public CustomerOrder2()
        {
            Invoices = new List<Invoice>();
        }
        public string NewField { get; set; }
        public ICollection<Invoice> Invoices { get; set; }
    }
}
