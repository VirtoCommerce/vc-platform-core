using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Notifications.NotificationTypes
{
    public class InvoiceEmailNotification : EmailNotification
    {
        public CustomerOrder Order { get; set; }
    }
}
