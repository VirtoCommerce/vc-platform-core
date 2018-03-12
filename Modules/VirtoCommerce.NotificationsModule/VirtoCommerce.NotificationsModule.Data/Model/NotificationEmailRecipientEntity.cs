using System.ComponentModel.DataAnnotations;
using VirtoCommerce.NotificationsModule.Data.Enums;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Model
{
    /// <summary>
    /// Entity is recipient of notification
    /// </summary>
    public class NotificationEmailRecipientEntity : Entity
    {
        /// <summary>
        /// Recipient info (e-mail and etc.) of notification
        /// </summary>
        [StringLength(128)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Type of recipient (CC, BCC and etc.)
        /// </summary>
        public NotificationRecipientType RecipientType { get; set; }

        /// <summary>
        /// Id of notification
        /// </summary>
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
