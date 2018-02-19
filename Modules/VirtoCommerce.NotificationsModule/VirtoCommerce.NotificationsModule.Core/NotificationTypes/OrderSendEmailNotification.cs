using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Core.NotificationTypes
{
    public class OrderSendEmailNotification : EmailNotification
    {
        public OrderSendEmailNotification()
        {
            Type = nameof(OrderSendEmailNotification);
        }

        public object Order { get; set; }
    }
}
