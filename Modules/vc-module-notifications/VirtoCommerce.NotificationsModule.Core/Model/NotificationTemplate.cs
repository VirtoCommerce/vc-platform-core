using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Core.Model
{
    public abstract class NotificationTemplate : AuditableEntity, IHasLanguageCode
    {
        public string LanguageCode { get; set; }
    }
}
