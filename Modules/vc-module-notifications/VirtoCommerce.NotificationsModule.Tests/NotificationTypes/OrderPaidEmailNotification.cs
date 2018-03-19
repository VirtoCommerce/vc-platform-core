using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Tests.Model;

namespace VirtoCommerce.NotificationsModule.Tests.NotificationTypes
{
    public class OrderPaidEmailNotification : EmailNotification
    {
        public CustomerOrder CustomerOrder { get; set; }
    }
}
