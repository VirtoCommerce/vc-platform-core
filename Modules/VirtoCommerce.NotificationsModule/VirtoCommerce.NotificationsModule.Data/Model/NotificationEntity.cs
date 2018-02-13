using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.NotificationsModule.Data.Model
{
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
        /// Copy Recipient
        /// </summary>
        [StringLength(128)]
        public string CC { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [StringLength(128)]
        public string BCC { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [StringLength(128)]
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
            notification.Kind = this.Kind;

            switch (notification)
            {
                case EmailNotification emailNotification:
                    emailNotification.From = this.From;
                    emailNotification.To = this.To;
                    //emailNotification.CC = this.CC.Split(';');
                    //emailNotification.BCC = this.BCC.Split(';');
                    break;
                case SmsNotification smsNotification:
                    smsNotification.Number = this.Number;
                    break;
            }

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
            this.Kind = notification.Kind;

            if (notification.Templates != null)
            {
                this.Templates = new ObservableCollection<NotificationTemplateEntity>(notification.Templates.Select(x => new NotificationTemplateEntity
                {
                    LanguageCode = x.LanguageCode,
                }));
            }

            switch (notification)
            {
                case EmailNotification emailNotification:
                    this.From = emailNotification.From;
                    this.To = emailNotification.To;
                    //this.CC = string.Join(";", emailNotification.CC);
                    //this.BCC = string.Join(";", emailNotification.BCC);
                    break;
                case SmsNotification smsNotification:
                    this.Number = smsNotification.Number;
                    break;
            }

            return this;
        }
    }
}
