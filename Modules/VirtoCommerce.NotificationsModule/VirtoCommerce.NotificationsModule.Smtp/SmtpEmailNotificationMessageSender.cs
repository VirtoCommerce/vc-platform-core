using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Smtp
{
    public class SmtpEmailNotificationMessageSender : INotificationMessageSender
    {
        private readonly EmailSendingOptions _emailSendingOptions;

        public SmtpEmailNotificationMessageSender(IOptions<EmailSendingOptions> emailSendingOptions)
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
                using (MailMessage mailMsg = new MailMessage())
                {
                    mailMsg.From = new MailAddress(emailNotificationMessage.From);
                    mailMsg.ReplyToList.Add(mailMsg.From);

                    mailMsg.Subject = emailNotificationMessage.Subject;
                    mailMsg.Body = emailNotificationMessage.Body;
                    mailMsg.IsBodyHtml = true;

                    using (var client = CreateClient())
                    {
                        await client.SendMailAsync(mailMsg);
                    }
                };

                result.IsSuccess = true;
            }
            catch (SmtpException ex)
            {
                result.ErrorMessage = ex.Message + ex.InnerException;
            }

            return result;
        }

        private SmtpClient CreateClient()
        {
            return new SmtpClient(_emailSendingOptions.SmtpServer, _emailSendingOptions.Port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(_emailSendingOptions.Login, _emailSendingOptions.Password)
            };
        }
    }
}
