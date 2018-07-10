using VirtoCommerce.OrderModule.Core.Model;

namespace VirtoCommerce.OrderModule.Core.Notifications
{
    public class InvoiceEmailNotification : OrderEmailNotificationBase
    {
        public CustomerOrder Order => CustomerOrder;
    }
}
