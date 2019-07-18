using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Model
{
    /// <summary>
    /// Entity is message of notification
    /// </summary>
    public abstract class NotificationMessageEntity : AuditableEntity, ICloneable
    {
        [NotMapped]
        public abstract string Kind { get; }

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

        #region Navigation Properties

        /// <summary>
        /// Id of notification
        /// </summary>
        public string NotificationId { get; set; }
        /// <summary>
        /// Notification property
        /// </summary>
        public NotificationEntity Notification { get; set; }

        #endregion

        public virtual NotificationMessage ToModel(NotificationMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            message.Id = Id;
            message.CreatedBy = CreatedBy;
            message.CreatedDate = CreatedDate;
            message.ModifiedBy = ModifiedBy;
            message.ModifiedDate = ModifiedDate;

            message.TenantIdentity = new TenantIdentity(TenantId, TenantType);
            message.NotificationId = NotificationId;
            message.NotificationType = NotificationType;
            message.SendAttemptCount = SendAttemptCount;
            message.MaxSendAttemptCount = MaxSendAttemptCount;
            message.LastSendError = LastSendError;
            message.LastSendAttemptDate = LastSendAttemptDate;
            message.SendDate = SendDate;
            message.LanguageCode = LanguageCode;

            return message;
        }

        public virtual NotificationMessageEntity FromModel(NotificationMessage message, PrimaryKeyResolvingMap pkMap)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            pkMap.AddPair(message, this);

            Id = message.Id;
            CreatedBy = message.CreatedBy;
            CreatedDate = message.CreatedDate;
            ModifiedBy = message.ModifiedBy;
            ModifiedDate = message.ModifiedDate;

            TenantId = message.TenantIdentity?.Id;
            TenantType = message.TenantIdentity?.Type;
            NotificationId = message.NotificationId;
            NotificationType = message.NotificationType;
            SendAttemptCount = message.SendAttemptCount;
            MaxSendAttemptCount = message.MaxSendAttemptCount;
            LastSendError = message.LastSendError;
            LastSendAttemptDate = message.LastSendAttemptDate;
            SendDate = message.SendDate;
            LanguageCode = message.LanguageCode;

            return this;
        }

        public virtual void Patch(NotificationMessageEntity message)
        {
            message.TenantId = TenantId;
            message.TenantType = TenantType;
            message.LanguageCode = LanguageCode;
            message.SendAttemptCount = SendAttemptCount;
            message.MaxSendAttemptCount = MaxSendAttemptCount;
            message.LastSendError = LastSendError;
            message.LastSendAttemptDate = LastSendAttemptDate;
            message.SendDate = SendDate;
        }

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as NotificationMessageEntity;

            if (Notification != null)
            {
                result.Notification = Notification.Clone() as NotificationEntity;
            }

            return result;
        }

        #endregion
    }
}
