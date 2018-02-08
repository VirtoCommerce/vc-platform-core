using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Core.Model
{
    public abstract class NotificationTemplate : AuditableEntity
    {
        public string LanguageCode { get; set; }
    }
}
