using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Model
{
    /// <summary>
    /// Entity is message of notification
    /// </summary>
    public class NotificationMessageEntity : AuditableEntity
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
        /// Id of notification
        /// </summary>
        [StringLength(128)]
        public string NotificationId { get; set; }

        /// <summary>
        /// Type of notification
        /// </summary>
        [StringLength(128)]
        public string NotificationType { get; set; }

        /// <summary>
        /// Number of current attemp
        /// </summary>
        public int SendAttemptCount { get; set; }

        /// <summary>
        /// Maximum number of attempts to send a message
        /// </summary>
        public int MaxSendAttemptCount { get; set; }

        /// <summary>
        /// Last fail attempt error message
        /// </summary>
        public string LastSendError { get; set; }

        /// <summary>
        /// Date of last fail attempt
        /// </summary>
        public DateTime? LastSendAttemptDate { get; set; }

        /// <summary>
        /// Date of start sending notification
        /// </summary>
        public DateTime? SendDate { get; set; }

        /// <summary>
        /// Language
        /// </summary>
        [StringLength(10)]
        public string LanguageCode { get; set; }

        /// <summary>
        /// Subject of notification
        /// </summary>
        [StringLength(512)]
        public string Subject { get; set; }

        /// <summary>
        /// Body of notification
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Message of notification
        /// </summary>
        [StringLength(1600)]
        public string Message { get; set; }

        /// <summary>
        /// Notification property
        /// </summary>
        public NotificationEntity Notification { get; set; }

        public virtual NotificationMessage ToModel(NotificationMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            message.Id = Id;
            message.TenantIdentity = new TenantIdentity(TenantId, TenantType);
            message.NotificationId = NotificationId;
            message.NotificationType = NotificationType;
            message.SendAttemptCount = SendAttemptCount;
            message.MaxSendAttemptCount = MaxSendAttemptCount;
            message.LastSendError = LastSendError;
            message.LastSendAttemptDate = LastSendAttemptDate;
            message.SendDate = SendDate;
            message.CreatedBy = CreatedBy;
            message.CreatedDate = CreatedDate;
            message.ModifiedBy = ModifiedBy;
            message.ModifiedDate = ModifiedDate;
            message.LanguageCode = LanguageCode;

            return message;
        }

        public virtual NotificationMessageEntity FromModel(NotificationMessage message, PrimaryKeyResolvingMap pkMap)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            pkMap.AddPair(message, this);

            Id = message.Id;
            TenantId = message.TenantIdentity?.Id;
            TenantType = message.TenantIdentity?.Type;
            NotificationId = message.NotificationId;
            NotificationType = message.NotificationType;
            SendAttemptCount = message.SendAttemptCount;
            MaxSendAttemptCount = message.MaxSendAttemptCount;
            LastSendError = message.LastSendError;
            LastSendAttemptDate = message.LastSendAttemptDate;
            SendDate = message.SendDate;
            CreatedBy = message.CreatedBy;
            CreatedDate = message.CreatedDate;
            ModifiedBy = message.ModifiedBy;
            ModifiedDate = message.ModifiedDate;
            LanguageCode = message.LanguageCode;

            return this;
        }

        public virtual void Patch(NotificationMessageEntity message)
        {
            message.TenantId = TenantId;
            message.TenantType = TenantType;
            message.Notification = Notification;
            message.Body = Body;
            message.LanguageCode = LanguageCode;
            message.NotificationId = NotificationId;
            message.NotificationType = NotificationType;
            message.SendAttemptCount = SendAttemptCount;
            message.MaxSendAttemptCount = MaxSendAttemptCount;
            message.LastSendError = LastSendError;
            message.LastSendAttemptDate = LastSendAttemptDate;
            message.SendDate = SendDate;
        }
    }
}
