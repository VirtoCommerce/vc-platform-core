using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.SendGrid
{
    public class SendGridEmailNotificationMessageSender : INotificationMessageSender
    {
        private readonly EmailSendingOptions _emailSendingOptions;

        public SendGridEmailNotificationMessageSender(IOptions<EmailSendingOptions> emailSendingOptions)
        {
            _emailSendingOptions = emailSendingOptions.Value;
        }

        public async Task<NotificationSendResult> SendNotificationAsync(NotificationMessage message)
        {
            var result = new NotificationSendResult();

            var emailNotificationMessage = message as EmailNotificationMessage;

            if (emailNotificationMessage == null) throw new ArgumentNullException(nameof(emailNotificationMessage));

            try
            {
                var client = new SendGridClient(_emailSendingOptions.ApiKey);
                var mailMsg = new SendGridMessage
                {
                    From = new EmailAddress(emailNotificationMessage.From),
                    Subject = emailNotificationMessage.Subject,
                    HtmlContent = emailNotificationMessage.Body
                };

                var response = await client.SendEmailAsync(mailMsg);
                result.IsSuccess = true;
            }
            catch (SmtpException ex)
            {
                result.ErrorMessage = ex.Message + ex.InnerException;
            }

            return result;
        }
    }
}
