using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsSampleModule.Web.Models
{
    public class TwitterNotification : Notification
    {
        public TwitterNotification()
        {
            Kind = nameof(TwitterNotification);
        }
    }
}
