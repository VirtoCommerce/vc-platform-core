using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.Platform.Core.Notifications;

namespace VirtoCommerce.NotificationsModule.Data.Senders
{
    public class NotificationEmailSender : IEmailSender
    {
        private readonly INotificationSender _notificationSender;
        private readonly INotificationService _notificationService;

        public NotificationEmailSender(INotificationSender notificationSender, INotificationService notificationService)
        {
            _notificationSender = notificationSender;
            _notificationService = notificationService;
        }

        public async Task SendEmailAsync(string to, string type, string languageCode, Dictionary<string, object> parameters)
        {
            var notification = await _notificationService.GetByTypeAsync(type);

            if (notification is EmailNotification emailNotification)
            {
                emailNotification.To = to;    
            }

            ParametersResolver(notification, parameters);

            await _notificationSender.SendNotificationAsync(notification, languageCode);
        }

        private void ParametersResolver(Notification notification, Dictionary<string, object> parameters)
        {
            if (parameters.Any())
            {
                var properties = notification.GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(NotificationParameterAttribute), true).Any());
                foreach (var property in properties)
                {
                    parameters.TryGetValue(property.Name, out object value);
                    property.SetValue(notification, value, null);
                }
            }
        }
    }
}
