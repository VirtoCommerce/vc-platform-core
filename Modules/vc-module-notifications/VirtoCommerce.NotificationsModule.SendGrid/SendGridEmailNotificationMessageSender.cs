using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using VirtoCommerce.NotificationsModule.Core;
using VirtoCommerce.NotificationsModule.Core.Exceptions;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;

namespace VirtoCommerce.NotificationsModule.SendGrid
{
    public class SendGridEmailNotificationMessageSender : INotificationMessageSender
    {
        private readonly EmailSendingOptions _emailSendingOptions;

        public SendGridEmailNotificationMessageSender(IOptions<EmailSendingOptions> emailSendingOptions)
        {
            _emailSendingOptions = emailSendingOptions.Value;
        }

        public async Task SendNotificationAsync(NotificationMessage message)
        {
            try
            {
                var emailNotificationMessage = message as EmailNotificationMessage;

                if (emailNotificationMessage == null)
                {
                    throw new ArgumentNullException(nameof(emailNotificationMessage));
                }

                var client = new SendGridClient(_emailSendingOptions.SendGridOptions.ApiKey);
                var mailMsg = new SendGridMessage
                {
                    From = new EmailAddress(emailNotificationMessage.From),
                    Subject = emailNotificationMessage.Subject,
                    HtmlContent = emailNotificationMessage.Body
                };

                await client.SendEmailAsync(mailMsg);
            }
            catch (SmtpException ex)
            {
                throw new SentNotificationException(ex.Message, ex);
            }
            
        }
    }
}
