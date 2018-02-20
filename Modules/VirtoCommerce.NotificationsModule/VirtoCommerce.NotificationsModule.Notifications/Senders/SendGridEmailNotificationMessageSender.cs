using System;
using System.Net.Mail;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Notifications.Senders
{
    public class SendGridEmailNotificationMessageSender : INotificationMessageSender
    {
        private readonly EmailSettings _emailSettings;

        public SendGridEmailNotificationMessageSender(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }

        public async Task SendNotificationAsync(NotificationMessage message)
        {
            var emailNotificationMessage = message as EmailNotificationMessage;

            if (emailNotificationMessage == null) throw new ArgumentNullException(nameof(emailNotificationMessage));

            try
            {
                var client = new SendGridClient(_emailSettings.ApiKey);
                var mailMsg = new SendGridMessage();
                mailMsg.From = new SendGrid.Helpers.Mail.EmailAddress(emailNotificationMessage.From);

                mailMsg.Subject = emailNotificationMessage.Subject;
                mailMsg.HtmlContent = emailNotificationMessage.Body;

                var response = await client.SendEmailAsync(mailMsg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
