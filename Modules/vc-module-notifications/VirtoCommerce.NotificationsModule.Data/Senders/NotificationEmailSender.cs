using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.Platform.Core.Notifications;

namespace VirtoCommerce.NotificationsModule.Data.Senders
{
    public class NotificationEmailSender : IEmailSender
    {
        private readonly INotificationMessageSenderProviderFactory _notificationMessageSenderProviderFactory;
        private readonly EmailSendingOptions _emailSendingOptions;

        public NotificationEmailSender(INotificationMessageSenderProviderFactory notificationMessageSenderProviderFactory, IOptions<EmailSendingOptions> emailSendingOptions/*, IConfiguration configuration*/)
        {
            _notificationMessageSenderProviderFactory = notificationMessageSenderProviderFactory;
            _emailSendingOptions = emailSendingOptions.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var notificationMessage = new EmailNotificationMessage()
            {
                From = _emailSendingOptions.DefaultSender,
                To = email,
                Subject = subject,
                Body = message
            };
            await _notificationMessageSenderProviderFactory.GetSenderForNotificationType(nameof(EmailNotification)).SendNotificationAsync(notificationMessage);
        }

        
    }
}
