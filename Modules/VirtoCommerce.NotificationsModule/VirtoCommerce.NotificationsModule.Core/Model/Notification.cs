using System.Collections.Generic;
using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Core.Model
{
    public abstract class Notification : AuditableEntity
    {
        public Notification()
        {
            Tenant = new Tenant();
        }
        public Tenant Tenant { get; set; }
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
            message.Tenant.Id = Tenant.Id;
            message.Tenant.Type = Tenant.Type;
            message.NotificationType = Type;

            return message;
        }
    }
}
