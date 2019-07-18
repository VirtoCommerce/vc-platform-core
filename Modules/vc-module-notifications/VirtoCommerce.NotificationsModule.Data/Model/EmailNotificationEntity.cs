using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Model
{
    public class EmailNotificationEntity : NotificationEntity, ICloneable
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

        #region Navigation Properties

        public virtual ObservableCollection<NotificationEmailRecipientEntity> Recipients { get; set; }
            = new NullCollection<NotificationEmailRecipientEntity>();

        public virtual ObservableCollection<EmailAttachmentEntity> Attachments { get; set; }
            = new NullCollection<EmailAttachmentEntity>();

        #endregion

        public override Notification ToModel(Notification notification)
        {
            base.ToModel(notification);

            var emailNotification = notification as EmailNotification;

            if (emailNotification != null)
            {
                emailNotification.From = From;
                emailNotification.To = To;
                if (!Recipients.IsNullOrEmpty())
                {
                    emailNotification.CC = Recipients.Where(r => r.RecipientType == NotificationRecipientType.Cc)
                        .Select(cc => cc.EmailAddress).ToArray();
                    emailNotification.BCC = Recipients.Where(r => r.RecipientType == NotificationRecipientType.Bcc)
                        .Select(bcc => bcc.EmailAddress).ToArray();
                }

                if (!Attachments.IsNullOrEmpty())
                {
                    emailNotification.Attachments = Attachments.Select(en =>
                        en.ToModel(AbstractTypeFactory<EmailAttachment>.TryCreateInstance())).ToList();
                }
            }

            return notification;
        }

        public override NotificationEntity FromModel(Notification notification, PrimaryKeyResolvingMap pkMap)
        {
            if (notification is EmailNotification emailNotification)
            {
                From = emailNotification.From;
                To = emailNotification.To;

                if (emailNotification.CC != null && emailNotification.CC.Any())
                {
                    if (Recipients.IsNullCollection()) Recipients = new ObservableCollection<NotificationEmailRecipientEntity>();
                    Recipients.AddRange(emailNotification.CC.Select(cc => AbstractTypeFactory<NotificationEmailRecipientEntity>.TryCreateInstance()
                        .FromModel(cc, NotificationRecipientType.Cc)));
                }

                if (emailNotification.BCC != null && emailNotification.BCC.Any())
                {
                    if (Recipients.IsNullCollection()) Recipients = new ObservableCollection<NotificationEmailRecipientEntity>();
                    Recipients.AddRange(emailNotification.BCC.Select(bcc => AbstractTypeFactory<NotificationEmailRecipientEntity>.TryCreateInstance()
                        .FromModel(bcc, NotificationRecipientType.Bcc)));
                }

                if (emailNotification.Attachments != null && emailNotification.Attachments.Any())
                {
                    Attachments = new ObservableCollection<EmailAttachmentEntity>(emailNotification.Attachments.Select(a =>
                        AbstractTypeFactory<EmailAttachmentEntity>.TryCreateInstance().FromModel(a)));
                }
            }

            return base.FromModel(notification, pkMap);
        }

        public override void Patch(NotificationEntity notification)
        {
            if (notification is EmailNotificationEntity emailNotification)
            {
                emailNotification.From = From;
                emailNotification.To = To;

                if (!Recipients.IsNullCollection())
                {
                    Recipients.Patch(emailNotification.Recipients, (source, target) => source.Patch(target));
                }

                if (!Attachments.IsNullCollection())
                {
                    Attachments.Patch(emailNotification.Attachments, (source, attachmentEntity) => source.Patch(attachmentEntity));
                }
            }

            base.Patch(notification);
        }

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as EmailNotificationEntity;

            if (Recipients != null)
            {
                result.Recipients = new ObservableCollection<NotificationEmailRecipientEntity>(
                    Recipients.Select(x => x.Clone() as NotificationEmailRecipientEntity));
            }

            if (Attachments != null)
            {
                result.Attachments = new ObservableCollection<EmailAttachmentEntity>(
                    Attachments.Select(x => x.Clone() as EmailAttachmentEntity));
            }

            return result;
        }

        #endregion
    }

    /// <summary>
    /// Type of recipient
    /// </summary>
    public enum NotificationRecipientType
    {
        Cc = 1,
        Bcc
    }
}
