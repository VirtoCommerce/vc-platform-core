using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Data.Enums;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Model
{
    public class EmailNotificationEntity : NotificationEntity
    {
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

        public virtual ObservableCollection<NotificationEmailRecipientEntity> Recipients { get; set; } = new NullCollection<NotificationEmailRecipientEntity>();

        public override Notification ToModel(Notification notification)
        {
            var emailNotification = notification as EmailNotification;

            if (emailNotification != null)
            {
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
            }

            return base.ToModel(notification);
        }

        public override NotificationEntity FromModel(Notification notification, PrimaryKeyResolvingMap pkMap)
        {
            var emailNotification = notification as EmailNotification;
            if (emailNotification != null)
            {
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
            }

            return base.FromModel(notification, pkMap);
        }

        public override void Patch(NotificationEntity notification)
        {
            var emailNotification = notification as EmailNotificationEntity;
            if (emailNotification != null)
            {
                emailNotification.From = this.From;
                emailNotification.To = this.To;

                if (!this.Recipients.IsNullCollection())
                {
                    this.Recipients.Patch(emailNotification.Recipients, (source, target) => source.Patch(target));
                }
            }

            base.Patch(notification);
        }
    }
}
