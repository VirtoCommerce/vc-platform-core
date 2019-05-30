using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Core.Types
{
    public class ResetPasswordEmailNotification : EmailNotification
    {
        public string Url { get; set; }
    }
}
