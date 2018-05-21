using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Core.Model
{
    /// <summary>
    /// Template of Notification with a deferent language
    /// </summary>
    public abstract class NotificationTemplate : AuditableEntity, IHasLanguageCode
    {
        /// <summary>
        /// Code of Language
        /// </summary>
        public string LanguageCode { get; set; }
    }
}
