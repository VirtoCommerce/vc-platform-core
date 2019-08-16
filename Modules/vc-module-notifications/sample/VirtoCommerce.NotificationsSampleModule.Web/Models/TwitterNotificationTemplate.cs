using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsSampleModule.Web.Models
{
    public class TwitterNotificationTemplate : NotificationTemplate
    {
        public override string Kind => nameof(TwitterNotification);
    }
}
