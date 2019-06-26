using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VirtoCommerce.NotificationsModule.Core.Model;
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
            notification.Id = Id;
            notification.CreatedBy = CreatedBy;
            notification.CreatedDate = CreatedDate;
            notification.ModifiedBy = ModifiedBy;
            notification.ModifiedDate = ModifiedDate;
            notification.OuterId = OuterId;

            notification.TenantIdentity = string.IsNullOrEmpty(TenantId) ? TenantIdentity.Empty : new TenantIdentity(TenantId, TenantType);
            notification.IsActive = IsActive;
            notification.Type = Type;

            notification.Templates = Templates
                .Select(t => t.ToModel(AbstractTypeFactory<NotificationTemplate>.TryCreateInstance($"{Kind}Template"))).ToList();

            return notification;
        }

        public virtual NotificationEntity FromModel(Notification notification, PrimaryKeyResolvingMap pkMap)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            pkMap.AddPair(notification, this);

            Id = notification.Id;
            CreatedBy = notification.CreatedBy;
            CreatedDate = notification.CreatedDate;
            ModifiedBy = notification.ModifiedBy;
            ModifiedDate = notification.ModifiedDate;
            OuterId = notification.OuterId;

            TenantId = notification.TenantIdentity?.Id;
            TenantType = notification.TenantIdentity?.Type;
            Type = notification.Type;
            IsActive = notification.IsActive;
            Kind = notification.Kind;

            if (notification.Templates != null && notification.Templates.Any())
            {
                Templates = new ObservableCollection<NotificationTemplateEntity>(notification.Templates
                        .Select(x => AbstractTypeFactory<NotificationTemplateEntity>.TryCreateInstance($"{Kind}TemplateEntity").FromModel(x, pkMap)));
            }

            return this;
        }

        public virtual void Patch(NotificationEntity notification)
        {
            notification.IsActive = IsActive;

            if (!Templates.IsNullCollection())
            {
                Templates.Patch(notification.Templates, (sourceTemplate, templateEntity) => sourceTemplate.Patch(templateEntity));
            }
        }
    }
}
