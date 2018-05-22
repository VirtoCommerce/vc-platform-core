using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Core.Types;
using VirtoCommerce.Platform.Core.Notifications;

namespace VirtoCommerce.NotificationsModule.Data.Senders
{
    public class NotificationEmailSender : IEmailSender
    {
        private readonly INotificationMessageSenderProviderFactory _notificationMessageAccessor;

        public NotificationEmailSender(INotificationMessageSenderProviderFactory notificationMessageSenderProviderFactory)
        {
            _notificationMessageAccessor = notificationMessageSenderProviderFactory;
        }

        public Task SendEmailAsync(string recipient, string subject, string message, params string[] parameters)
        {

            var notification = new ResetPasswordEmailNotification();

            if (parameters.Any())
            {
                
            }
        }
    }
}
