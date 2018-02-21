using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Notifications.NotificationTypes
{
    public class ConfirmationEmailNotification : EmailNotification
    {
        public ConfirmationEmailNotification()
        {
            Type = nameof(ConfirmationEmailNotification);
        }
    }
}
