using System.Collections.Generic;
using VirtoCommerce.NotificationsModule.Core.Extensions;
using VirtoCommerce.NotificationsModule.Core.Services;

namespace VirtoCommerce.NotificationsModule.Core.Model
{
    /// <summary>
    /// Sms - Kind of Notification
    /// </summary>
    public abstract class SmsNotification : Notification
    {
        public SmsNotification()
        {
            Templates = new List<NotificationTemplate>();
        }


        public override string Kind => nameof(SmsNotification);

        /// <summary>
        /// Number for sms notification
        /// </summary>
        public string Number { get; set; }

        public override void ToMessage(NotificationMessage message, INotificationTemplateRenderer render)
        {
            base.ToMessage(message, render);

            var smsNotificationMessage = (SmsNotificationMessage)message;
            var template = (SmsNotificationTemplate)Templates.FindWithLanguage(message.LanguageCode);
            if (template != null)
            {
                smsNotificationMessage.Message = render.RenderAsync(template.Message, this, template.LanguageCode).GetAwaiter().GetResult();
            }
            smsNotificationMessage.Number = Number;
        }

        public override void SetFromToMembers(string from, string to)
        {
            Number = to;
        }

        #region ICloneable members

        public override object Clone()
        {
            return base.Clone() as SmsNotification;
        }

        #endregion
    }
}
