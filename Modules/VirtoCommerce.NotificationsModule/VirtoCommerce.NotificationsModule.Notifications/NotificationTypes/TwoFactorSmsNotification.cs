using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Notifications.NotificationTypes
{
    public class TwoFactorSmsNotification : SmsNotification
    {
        public TwoFactorSmsNotification()
        {
            Type = nameof(TwoFactorSmsNotification);
        }
    }
}
