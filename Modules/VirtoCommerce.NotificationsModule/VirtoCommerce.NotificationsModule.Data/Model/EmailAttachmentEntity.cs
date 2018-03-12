using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Model
{
    /// <summary>
    /// Entity is attachment of email
    /// </summary>
    public class EmailAttachmentEntity : AuditableEntity
    {
        /// <summary>
        /// Name of Attachment
        /// </summary>
        [StringLength(512)]
        public string FileName { get; set; }

        /// <summary>
        /// Url of Attachment
        /// </summary>
        [StringLength(2048)]
        public string Url { get; set; }

        /// <summary>
        /// MimeType of Attachment
        /// </summary>
        [StringLength(50)]
        public string MimeType { get; set; }

        /// <summary>
        /// Size of Attachment
        /// </summary>
        [StringLength(128)]
        public string Size { get; set; }

        /// <summary>
        /// Language of Attachment
        /// </summary>
        [StringLength(10)]
        public string LanguageCode { get; set; }

        /// <summary>
        /// Id of notification
        /// </summary>
        [StringLength(128)]
        public string NotificationId { get; set; }

        public virtual EmailAttachment ToModel(EmailAttachment emailAttachment)
        {
            if (emailAttachment == null) throw new ArgumentNullException(nameof(emailAttachment));
            emailAttachment.FileName = this.FileName;
            emailAttachment.Url = this.Url;
            emailAttachment.MimeType = this.MimeType;
            emailAttachment.Size = this.Size;
            emailAttachment.LanguageCode = this.LanguageCode;

            return emailAttachment;
        }

        public virtual EmailAttachmentEntity FromModel(EmailAttachment emailAttachment)
        {
            if (emailAttachment == null) throw new ArgumentNullException(nameof(emailAttachment));
            this.FileName = emailAttachment.FileName;
            this.Url = emailAttachment.Url;
            this.MimeType = emailAttachment.MimeType;
            this.Size = emailAttachment.Size;
            this.LanguageCode = emailAttachment.LanguageCode;

            return this;
        }

        public void Patch(EmailAttachmentEntity attachment)
        {
            attachment.LanguageCode = this.LanguageCode;
            attachment.FileName = this.FileName;
            attachment.MimeType = this.MimeType;
            attachment.Size = this.Size;
            attachment.Url = this.Url;
        }
    }
}
