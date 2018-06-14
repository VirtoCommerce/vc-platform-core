using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsSampleModule.Web.Models
{
    public class TwitterNotification : Notification
    {
        public string Post { get; set; }
        public TwitterNotification()
        {
            Kind = nameof(TwitterNotification);
        }
    }
}
