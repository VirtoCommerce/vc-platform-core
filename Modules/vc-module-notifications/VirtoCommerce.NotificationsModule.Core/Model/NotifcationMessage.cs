using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Core.Model
{
    public abstract class NotificationMessage : AuditableEntity, IHasLanguageCode
    {
        public NotificationMessage()
        {
            Tenant = new Tenant();
        }

        public Tenant Tenant { get; set; }
        public string NotificationId { get; set; }
        public string NotificationType { get; set; }
        public int SendAttemptCount { get; set; }
        public int MaxSendAttemptCount { get; set; }
        public string LastSendError { get; set; }
        public DateTime? LastSendAttemptDate { get; set; }
        public DateTime? SendDate { get; set; }
        public string LanguageCode { get; set; }
    }
}
