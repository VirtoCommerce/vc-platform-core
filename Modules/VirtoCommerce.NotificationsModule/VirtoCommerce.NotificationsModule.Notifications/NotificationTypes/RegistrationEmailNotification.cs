using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Notifications.NotificationTypes
{
    public class RegistrationEmailNotification : EmailNotification
    {
        public RegistrationEmailNotification()
        {
            Type = nameof(RegistrationEmailNotification);
        }
    }
}
