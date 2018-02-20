using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Notifications.NotificationTypes
{
    public class OrderSentEmailNotification : EmailNotification
    {
        public OrderSentEmailNotification()
        {
            Type = nameof(OrderSentEmailNotification);
        }

        public CustomerOrder Order { get; set; }
    }

    public class CustomerOrder
    {
        public string Id { get; set; }
    }
}
