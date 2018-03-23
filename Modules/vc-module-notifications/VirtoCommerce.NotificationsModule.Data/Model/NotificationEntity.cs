using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Data.Enums;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Model
{
    /// <summary>
    /// Entity is Notification
    /// </summary>
    public abstract class NotificationEntity : AuditableEntity
    {
        /// <summary>
        /// Tenant id that initiate sending
        /// </summary>
        [StringLength(128)]
        public string TenantId { get; set; }

        /// <summary>
        /// Tenant type that initiate sending
        /// </summary>
        [StringLength(128)]
        public string TenantType { get; set; }

        /// <summary>
        /// Must be made sending
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Type of notification
        /// </summary>
        [StringLength(128)]
        public string Type { get; set; }

        /// <summary>
        /// Type of notification
        /// </summary>
        [StringLength(128)]
        public string Kind { get; set; }

        public virtual ObservableCollection<NotificationTemplateEntity> Templates { get; set; } = new NullCollection<NotificationTemplateEntity>();
        
        public virtual Notification ToModel(Notification notification)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));
            notification.Id = this.Id;
            notification.Tenant.Id = this.TenantId;
            notification.Tenant.Type = this.TenantType;
            notification.IsActive = this.IsActive;
            notification.Type = this.Type;
            notification.CreatedBy = this.CreatedBy;
            notification.CreatedDate = this.CreatedDate;
            notification.ModifiedBy = this.ModifiedBy;
            notification.ModifiedDate = this.ModifiedDate;
            notification.Kind = this.Kind;

            notification.Templates = this.Templates
                .Select(t => t.ToModel(AbstractTypeFactory<NotificationTemplate>.TryCreateInstance($"{this.Kind}Template"))).ToList();

            return notification;
        }

        public virtual NotificationEntity FromModel(Notification notification, PrimaryKeyResolvingMap pkMap)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            pkMap.AddPair(notification, this);

            this.Id = notification.Id;
            this.TenantId = notification.Tenant.Id;
            this.TenantType = notification.Tenant.Type;
            this.Type = notification.Type;
            this.IsActive = notification.IsActive;
            this.CreatedBy = notification.CreatedBy;
            this.CreatedDate = notification.CreatedDate;
            this.ModifiedBy = notification.ModifiedBy;
            this.ModifiedDate = notification.ModifiedDate;
            this.Kind = notification.Kind;

            if (notification.Templates != null && notification.Templates.Any())
            {
                this.Templates = new ObservableCollection<NotificationTemplateEntity>(notification.Templates
                        .Select(x => AbstractTypeFactory<NotificationTemplateEntity>.TryCreateInstance().FromModel(x)));
            }

            return this;
        }

        public virtual void Patch(NotificationEntity notification)
        {
            notification.Type = this.Type;
            notification.Kind = this.Kind;
            notification.IsActive = this.IsActive;
            
            if (!this.Templates.IsNullCollection())
            {
                this.Templates.Patch(notification.Templates, (sourceTemplate, templateEntity) => sourceTemplate.Patch(templateEntity));
            }

            
        }
    }
}
