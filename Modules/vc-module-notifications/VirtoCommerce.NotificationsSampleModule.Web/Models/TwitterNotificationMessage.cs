using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsSampleModule.Web.Models
{

    public class TwitterNotificationMessage : NotificationMessage
    {
        public override string Kind => nameof(TwitterNotification);
    }
}
