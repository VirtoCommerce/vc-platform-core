using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Data.Enums;
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

        public NotificationRecipientType RecipientType { get; set; }

        [StringLength(128)]
        public string NotificationId { get; set; }

        public virtual NotificationEmailRecipientEntity FromModel(string emailAddress, NotificationRecipientType recipientType)
        {
            this.EmailAddress = emailAddress;
            this.RecipientType = recipientType;

            return this;
        }

        public virtual void Patch(NotificationEmailRecipientEntity emailAddress)
        {
            emailAddress.EmailAddress = this.EmailAddress;
        }
    }
}
