using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.OrderModule.Core.Model;

namespace VirtoCommerce.OrderModule.Core.Notifications
{
    public abstract class OrderEmailNotificationBase : EmailNotification
    {
        public virtual CustomerOrder CustomerOrder { get; set; }
    }
}
