using System;
using System.Threading.Tasks;
using System.Net.Mail;
using VirtoCommerce.NotificationsModule.Core.Model;

namespace VirtoCommerce.NotificationsModule.Notifications.Senders
{
    public class SmtpEmailNotificationMessageSender : INotificationMessageSender
    {
        private readonly EmailSettings _emailSettings;

        public SmtpEmailNotificationMessageSender(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }

        public async Task SendNotificationAsync(NotificationMessage message)
        {
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

                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private SmtpClient CreateClient()
        {
            return new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(_emailSettings.Login, _emailSettings.Password)
            };
        }
    }
}
