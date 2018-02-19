using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Core.NotificationTypes
{
    public class RegistrationEmailNotification : EmailNotification
    {
        public RegistrationEmailNotification()
        {
            Type = nameof(RegistrationEmailNotification);
        }
    }
}
