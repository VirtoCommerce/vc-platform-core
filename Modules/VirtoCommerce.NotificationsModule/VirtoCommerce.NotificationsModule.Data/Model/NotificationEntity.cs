using System;
using System.Collections.ObjectModel;
using System.Linq;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Model
{
    public class NotificationEntity : AuditableEntity
    {
        public string TenantId { get; set; }
        public string TenantType { get; set; }
        public bool IsActive { get; set; }
        public string Type { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public string Number { get; set; }

        public ObservableCollection<NotificationTemplateEntity> Templates { get; set; }
        public ObservableCollection<EmailAttachmentEntity> Attachments { get; set; }

        public virtual Notification ToModel(Notification notification)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));
            notification.Id = this.Id;
            notification.TenantId = this.TenantId;
            notification.TenantType = this.TenantType;
            notification.IsActive = this.IsActive;
            notification.Type = this.Type;
            notification.CreatedBy = this.CreatedBy;
            notification.CreatedDate = this.CreatedDate;
            notification.ModifiedBy = this.ModifiedBy;
            notification.ModifiedDate = this.ModifiedDate;

            return notification;
        }

        public virtual NotificationEntity FromModel(Notification notification)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            this.Id = notification.Id;
            this.TenantId = notification.TenantId;
            this.TenantType = notification.TenantType;
            this.Type = notification.Type;
            this.IsActive = notification.IsActive;
            this.CreatedBy = notification.CreatedBy;
            this.CreatedDate = notification.CreatedDate;
            this.ModifiedBy = notification.ModifiedBy;
            this.ModifiedDate = notification.ModifiedDate;

            if (notification.Templates != null)
            {
                this.Templates = new ObservableCollection<NotificationTemplateEntity>(notification.Templates.Select(x => new NotificationTemplateEntity
                {
                    LanguageCode = x.LanguageCode,
                }));
            }

            return this;
        }
    }
}
