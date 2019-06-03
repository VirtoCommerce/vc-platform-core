using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Core.Model
{
    /// <summary>
    /// A parent class for message of a notification with information about sending
    /// </summary>
    public abstract class NotificationMessage : AuditableEntity, IHasLanguageCode
    {
        public abstract string Kind { get; }

        /// <summary>
        /// For detecting owner
        /// </summary>
        public TenantIdentity TenantIdentity { get; set; }

        /// <summary>
        /// Id of Notification
        /// </summary>
        public string NotificationId { get; set; }

        /// <summary>
        /// Type of Notification
        /// </summary>
        public string NotificationType { get; set; }

        /// <summary>
        /// Count of sending attempt
        /// </summary>
        public int SendAttemptCount { get; set; }

        /// <summary>
        /// Max count of sending attempt
        /// </summary>
        public int MaxSendAttemptCount { get; set; }

        /// <summary>
        /// The last error of sending
        /// </summary>
        public string LastSendError { get; set; }

        /// <summary>
        /// The last date of sending attempt
        /// </summary>
        public DateTime? LastSendAttemptDate { get; set; }

        /// <summary>
        /// Date of sending
        /// </summary>
        public DateTime? SendDate { get; set; }

        /// <summary>
        /// Code of language
        /// </summary>
        public string LanguageCode { get; set; }
    }
}
