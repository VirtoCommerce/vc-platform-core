using System;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Model
{
    public class NotificationTemplateEntity : AuditableEntity
    {
        public string LanguageCode { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Message { get; set; }

        public NotificationTemplate ToModel(NotificationTemplate message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            message.Id = this.Id;
            message.LanguageCode = this.LanguageCode;
            message.CreatedBy = this.CreatedBy;
            message.CreatedDate = this.CreatedDate;
            message.ModifiedBy = this.ModifiedBy;
            message.ModifiedDate = this.ModifiedDate;

            return message;
        }

        public NotificationTemplateEntity FromModel(NotificationTemplate message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            this.Id = message.Id;
            this.LanguageCode = message.LanguageCode;
            this.CreatedBy = message.CreatedBy;
            this.CreatedDate = message.CreatedDate;
            this.ModifiedBy = message.ModifiedBy;
            this.ModifiedDate = message.ModifiedDate;

            return this;
        }
    }
}
