using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Tests.NotificationTypes
{
    public class InvoiceEmailNotification : EmailNotification
    {
        public CustomerOrder Order { get; set; }
    }
}
