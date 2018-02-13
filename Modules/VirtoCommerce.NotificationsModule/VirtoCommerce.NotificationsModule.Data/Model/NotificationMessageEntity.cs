using System;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Model
{
    public class NotificationMessageEntity : AuditableEntity
    {
        public string TenantId { get; set; }
        public string TenantType { get; set; }
        public string NotificationId { get; set; }
        public string NotificationType { get; set; }
        public int SendAttemptCount { get; set; }
        public int MaxSendAttemptCount { get; set; }
        public string LastSendError { get; set; }
        public DateTime? LastSendAttemptDate { get; set; }
        public DateTime? SendDate { get; set; }
        public string LanguageCode { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Message { get; set; }

        public NotificationEntity Notification { get; set; }

        public NotifcationMessage ToModel(NotifcationMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            message.Id = this.Id;
            message.TenantId = this.TenantId;
            message.TenantType = this.TenantType;
            message.NotificationId = this.NotificationId;
            message.NotificationType = this.NotificationType;
            message.SendAttemptCount = this.SendAttemptCount;
            message.MaxSendAttemptCount = this.MaxSendAttemptCount;
            message.LastSendError = this.LastSendError;
            message.LastSendAttemptDate = this.LastSendAttemptDate;
            message.SendDate = this.SendDate;
            message.CreatedBy = this.CreatedBy;
            message.CreatedDate = this.CreatedDate;
            message.ModifiedBy = this.ModifiedBy;
            message.ModifiedDate = this.ModifiedDate;

            return message;
        }

        public NotificationMessageEntity FromModel(NotifcationMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            this.Id = message.Id;
            this.TenantId = message.TenantId;
            this.TenantType = message.TenantType;
            this.NotificationId = message.NotificationId;
            this.NotificationType = message.NotificationType;
            this.SendAttemptCount = message.SendAttemptCount;
            this.MaxSendAttemptCount = message.MaxSendAttemptCount;
            this.LastSendError = message.LastSendError;
            this.LastSendAttemptDate = message.LastSendAttemptDate;
            this.SendDate = message.SendDate;
            this.CreatedBy = message.CreatedBy;
            this.CreatedDate = message.CreatedDate;
            this.ModifiedBy = message.ModifiedBy;
            this.ModifiedDate = message.ModifiedDate;

            return this;
        }
    }
}
