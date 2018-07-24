using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.OrdersModule2.Web.Model
{
    public class Invoice : OrderOperation
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }

    }
}
