using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Core.Types
{
    public class ResetPasswordEmailNotification : EmailNotification
    {
        [NotificationParameter("Reset password URL")]
        public string Url { get; set; }
    }
}
