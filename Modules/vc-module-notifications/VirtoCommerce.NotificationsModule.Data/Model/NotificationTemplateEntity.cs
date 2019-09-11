using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Model
{
    /// <summary>
    /// Entity is template of Notification
    /// </summary>
    public abstract class NotificationTemplateEntity : AuditableEntity
    {
        public abstract string Kind { get; }
        /// <summary>
        /// Language of template
        /// </summary>
        [StringLength(10)]
        public string LanguageCode { get; set; }

        #region Navigation Properties

        /// <summary>
        /// Id of notification
        /// </summary>
        [StringLength(128)]
        public string NotificationId { get; set; }
        public NotificationEntity Notification { get; set; }

        #endregion

        public virtual NotificationTemplate ToModel(NotificationTemplate template)
        {
            if (template == null) throw new ArgumentNullException(nameof(template));

            template.Id = Id;
            template.LanguageCode = LanguageCode;
            template.CreatedBy = CreatedBy;
            template.CreatedDate = CreatedDate;
            template.ModifiedBy = ModifiedBy;
            template.ModifiedDate = ModifiedDate;

            return template;
        }

        public virtual NotificationTemplateEntity FromModel(NotificationTemplate template, PrimaryKeyResolvingMap pkMap)
        {
            if (template == null) throw new ArgumentNullException(nameof(template));

            pkMap.AddPair(template, this);

            Id = template.Id;
            LanguageCode = template.LanguageCode;
            CreatedBy = template.CreatedBy;
            CreatedDate = template.CreatedDate;
            ModifiedBy = template.ModifiedBy;
            ModifiedDate = template.ModifiedDate;

            return this;
        }

        public virtual void Patch(NotificationTemplateEntity template)
        {
            template.LanguageCode = LanguageCode;
        }        
    }
}
