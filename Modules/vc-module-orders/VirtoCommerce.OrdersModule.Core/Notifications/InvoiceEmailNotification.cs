using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.OrdersModule.Core.Notifications
{
    public class InvoiceEmailNotification : OrderEmailNotificationBase
    {
        public CustomerOrder Order => CustomerOrder;
    }
}
