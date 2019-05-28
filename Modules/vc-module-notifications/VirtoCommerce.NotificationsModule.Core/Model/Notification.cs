using System.Collections.Generic;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Core.Model
{
    /// <summary>
    /// A parent class for Notification
    /// </summary>
    public abstract class Notification : AuditableEntity
    {
        /// <summary>
        /// For detecting owner
        /// </summary>
        public TenantIdentity TenantIdentity { get; set; } = TenantIdentity.Empty;
        public bool IsActive { get; set; }

        /// <summary>
        /// Type of notifications, like Identifier
        /// </summary>
        private string _type;
        public virtual string Type
        {
            get => !string.IsNullOrEmpty(_type) ? _type : GetType().Name;
            set => _type = value;
        }

        /// <summary>
        /// For detecting kind of notifications (email, sms and etc.)
        /// </summary>
        public abstract string Kind { get; }
        public IList<NotificationTemplate> Templates { get; set; }

        public virtual NotificationMessage ToMessage(NotificationMessage message, INotificationTemplateRenderer render)
        {
            message.TenantIdentity = new TenantIdentity(TenantIdentity?.Id, TenantIdentity?.Type);
            message.NotificationType = Type;
            message.NotificationId = Id;

            return message;
        }
    }
}
