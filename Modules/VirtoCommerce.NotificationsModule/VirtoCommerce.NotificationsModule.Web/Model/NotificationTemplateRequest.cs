using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Web.Model
{
    public class NotificationTemplateRequest
    {
        public string Text { get; set; }
        public Notification Data { get; set; }
    }
}
