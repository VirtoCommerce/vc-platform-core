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
    /// Model of 
    /// </summary>
    public class NotificationEntity : AuditableEntity
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

        /// <summary>
        /// Sender info (e-mail, phone number and etc.) of notification
        /// </summary>
        [StringLength(128)]
        public string From { get; set; }

        /// <summary>
        /// Recipient info (e-mail, phone number and etc.) of notification
        /// </summary>
        [StringLength(128)]
        public string To { get; set; }

        /// <summary>
        /// Number for sms
        /// </summary>
        [StringLength(128)]
        public string Number { get; set; }

        public virtual ObservableCollection<NotificationTemplateEntity> Templates { get; set; } = new NullCollection<NotificationTemplateEntity>();
        public virtual ObservableCollection<EmailAttachmentEntity> Attachments { get; set; } = new NullCollection<EmailAttachmentEntity>();
        public virtual ObservableCollection<NotificationEmailRecipientEntity> Recipients { get; set; } = new NullCollection<NotificationEmailRecipientEntity>();

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

            switch (notification)
            {
                case EmailNotification emailNotification:
                    emailNotification.From = this.From;
                    emailNotification.To = this.To;

                    if (!this.Recipients.IsNullOrEmpty())
                    {
                        emailNotification.CC = this.Recipients.Where(r => r.RecipientType == NotificationRecipientType.Cc)
                            .Select(cc => cc.EmailAddress).ToArray();
                        emailNotification.BCC = this.Recipients.Where(r => r.RecipientType == NotificationRecipientType.Bcc)
                            .Select(bcc => bcc.EmailAddress).ToArray();
                    }

                    if (!this.Attachments.IsNullOrEmpty())
                    {
                        emailNotification.Attachments = this.Attachments.Select(en =>
                            en.ToModel(AbstractTypeFactory<EmailAttachment>.TryCreateInstance())).ToList();
                    }
                    break;
                case SmsNotification smsNotification:
                    smsNotification.Number = this.Number;
                    break;
            }

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

            switch (notification)
            {
                case EmailNotification emailNotification:
                    this.From = emailNotification.From;
                    this.To = emailNotification.To;

                    if (emailNotification.CC != null && emailNotification.CC.Any())
                    {
                        if (this.Recipients.IsNullCollection()) this.Recipients = new ObservableCollection<NotificationEmailRecipientEntity>();
                        this.Recipients.AddRange(emailNotification.CC.Select(cc => AbstractTypeFactory<NotificationEmailRecipientEntity>.TryCreateInstance()
                                .FromModel(cc, NotificationRecipientType.Cc)));
                    }

                    if (emailNotification.BCC != null && emailNotification.BCC.Any())
                    {
                        if (this.Recipients.IsNullCollection()) this.Recipients = new ObservableCollection<NotificationEmailRecipientEntity>();
                        this.Recipients.AddRange(emailNotification.BCC.Select(bcc => AbstractTypeFactory<NotificationEmailRecipientEntity>.TryCreateInstance()
                                .FromModel(bcc, NotificationRecipientType.Bcc)));
                    }

                    if (emailNotification.Attachments != null && emailNotification.Attachments.Any())
                    {
                        this.Attachments = new ObservableCollection<EmailAttachmentEntity>(emailNotification.Attachments.Select(a =>
                            AbstractTypeFactory<EmailAttachmentEntity>.TryCreateInstance().FromModel(a)));
                    }
                    break;
                case SmsNotification smsNotification:
                    this.Number = smsNotification.Number;
                    break;
            }

            return this;
        }

        public virtual void Patch(NotificationEntity notification)
        {
            notification.Type = this.Type;
            notification.Kind = this.Kind;
            notification.IsActive = this.IsActive;
            notification.From = this.From;
            notification.To = this.To;
            notification.Number = this.Number;

            if (!this.Templates.IsNullCollection())
            {
                this.Templates.Patch(notification.Templates, (sourceTemplate, templateEntity) => sourceTemplate.Patch(templateEntity));
            }

            if (!this.Attachments.IsNullCollection())
            {
                this.Attachments.Patch(notification.Attachments, (source, attachmentEntity) => source.Patch(attachmentEntity));
            }

            if (!this.Recipients.IsNullCollection())
            {
                this.Recipients.Patch(notification.Recipients, (source, target) => source.Patch(target));
            }
        }
    }
}
