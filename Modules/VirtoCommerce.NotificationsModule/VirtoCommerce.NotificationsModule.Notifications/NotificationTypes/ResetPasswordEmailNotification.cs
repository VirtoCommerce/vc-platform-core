using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Notifications.NotificationTypes
{
    public class ResetPasswordEmailNotification : EmailNotification
    {
        public ResetPasswordEmailNotification()
        {
            Type = nameof(ResetPasswordEmailNotification);
        }
    }
}
