using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Core.Model
{
    /// <summary>
    /// Base class for Notification
    /// </summary>
    public abstract class Notification : AuditableEntity, ICloneable
    {
        /// <summary>
        /// For detecting owner
        /// </summary>
        public TenantIdentity TenantIdentity { get; set; } = TenantIdentity.Empty;
        public bool IsActive { get; set; }

        /// <summary>
        /// Type of notifications, like Identifier
        /// </summary>
        private string _type;
        public virtual string Type
        {
            get => !string.IsNullOrEmpty(_type) ? _type : GetType().Name;
            set => _type = value;
        }

        /// <summary>
        /// For detecting kind of notifications (email, sms and etc.)
        /// </summary>
        public abstract string Kind { get; }
        public IList<NotificationTemplate> Templates { get; set; }

        public virtual NotificationMessage ToMessage(NotificationMessage message, INotificationTemplateRenderer render)
        {
            message.TenantIdentity = new TenantIdentity(TenantIdentity?.Id, TenantIdentity?.Type);
            message.NotificationType = Type;
            message.NotificationId = Id;

            return message;
        }

        public virtual void ReduceDetails(string responseGroup)
        {
            //Reduce details according to response group
            var notificationResponseGroup = EnumUtility.SafeParseFlags(responseGroup, NotificationResponseGroup.Full);

            if (!notificationResponseGroup.HasFlag(NotificationResponseGroup.WithTemplates))
            {
                Templates = null;
            }
        }

        public abstract void SetFromToMembers(string from, string to);

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as Notification;

            if (Templates != null)
            {
                result.Templates = new ObservableCollection<NotificationTemplate>(
                    Templates.Select(x => x.Clone() as NotificationTemplate));
            }

            result.TenantIdentity = TenantIdentity?.Clone() as TenantIdentity;

            return result;
        }

        #endregion
    }
}
