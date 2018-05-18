using System.Collections.Generic;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Core.Model
{
    public abstract class Notification : AuditableEntity
    {
        public TenantIdentity TenantIdentity { get; set; }
        public bool IsActive { get; set; }

        private string _type;
        public string Type
        {
            get => !string.IsNullOrEmpty(_type) ? _type : this.GetType().Name;
            set => _type = value;
        }

        public string Kind { get; set; }
        public IList<NotificationTemplate> Templates { get; set; }

        public virtual NotificationMessage ToMessage(NotificationMessage message, INotificationTemplateRender render)
        {
            message.TenantIdentity = new TenantIdentity(message.TenantIdentity?.Id, message.TenantIdentity?.Type);
            message.NotificationType = Type;

            return message;
        }
    }
}
