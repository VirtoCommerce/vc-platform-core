using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Core.Model
{
    /// <summary>
    /// Template of Notification with a different language
    /// </summary>
    public abstract class NotificationTemplate : AuditableEntity, IHasLanguageCode, ICloneable
    {
        /// <summary>
        /// Code of Language
        /// </summary>
        public string LanguageCode { get; set; }
        /// <summary>
        /// For detecting kind of notifications (email, sms and etc.)
        /// </summary>
        public abstract string Kind { get; }

        public bool IsReadonly { get; set; }

        #region ICloneable members

        public virtual object Clone()
        {
            return MemberwiseClone() as NotificationTemplate;
        }

        #endregion
    }
}
