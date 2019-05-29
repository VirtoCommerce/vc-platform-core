using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsSampleModule.Web.Models
{
    public abstract class TwitterNotification : Notification
    {
        public string Post { get; set; }
        public override string Kind => nameof(TwitterNotification);
    }
}
