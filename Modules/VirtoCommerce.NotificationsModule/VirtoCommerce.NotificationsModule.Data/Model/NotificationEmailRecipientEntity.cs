using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Model
{
    public class NotificationEmailRecipientEntity : Entity
    {
        /// <summary>
        /// Recipient info (e-mail, phone number and etc.) of notification
        /// </summary>
        [StringLength(128)]
        public string EmailAddress { get; set; }

        public virtual EmailAddress ToModel(EmailAddress emailAddress)
        {
            if (emailAddress == null) throw new ArgumentNullException(nameof(emailAddress));

            emailAddress.Value = EmailAddress;
            emailAddress.Id = this.Id;

            return emailAddress;
        }

        public virtual NotificationEmailRecipientEntity FromModel(EmailAddress emailAddress)
        {
            if (emailAddress == null) throw new ArgumentNullException(nameof(emailAddress));

            this.EmailAddress = emailAddress.Value;
            this.Id = emailAddress.Id;

            return this;
        }

        public virtual void Patch(NotificationEmailRecipientEntity emailAddress)
        {
            emailAddress.EmailAddress = this.EmailAddress;
        }
    }
}
