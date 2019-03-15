using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VirtoCommerce.NotificationsModule.Core.Exceptions;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Smtp
{
    public class SmtpEmailNotificationMessageSender : INotificationMessageSender
    {
        private readonly SmtpSenderOptions _emailSendingOptions;

        public SmtpEmailNotificationMessageSender(IOptions<SmtpSenderOptions> emailSendingOptions)
        {
            _emailSendingOptions = emailSendingOptions.Value;
        }

        public async Task SendNotificationAsync(NotificationMessage message)
        {
            var emailNotificationMessage = message as EmailNotificationMessage;

            if (emailNotificationMessage == null)
            {
                throw new ArgumentNullException(nameof(emailNotificationMessage));
            }

            try
            {
                using (MailMessage mailMsg = new MailMessage())
                {
                    mailMsg.From = new MailAddress(emailNotificationMessage.From);
                    mailMsg.To.Add(new MailAddress(emailNotificationMessage.To));
                    mailMsg.ReplyToList.Add(mailMsg.From);

                    mailMsg.Subject = emailNotificationMessage.Subject;
                    mailMsg.Body = emailNotificationMessage.Body;
                    mailMsg.IsBodyHtml = true;

                    if (!emailNotificationMessage.CC.IsNullOrEmpty())
                    {
                        foreach (var ccEmail in emailNotificationMessage.CC)
                        {
                            mailMsg.CC.Add(new MailAddress(ccEmail));
                        }
                    }

                    if (!emailNotificationMessage.BCC.IsNullOrEmpty())
                    {
                        foreach (var bccEmail in emailNotificationMessage.BCC)
                        {
                            mailMsg.Bcc.Add(new MailAddress(bccEmail));
                        }
                    }

                    foreach (var attachment in emailNotificationMessage.Attachments)
                    {
                        mailMsg.Attachments.Add(new Attachment(attachment.FileName, attachment.MimeType));
                    }

                    using (var client = CreateClient())
                    {
                        await client.SendMailAsync(mailMsg);
                    }
                };
            }
            catch (SmtpException ex)
            {
                throw new SentNotificationException(ex.Message, ex);
            }
        }

        private SmtpClient CreateClient()
        {
            return new SmtpClient(_emailSendingOptions.SmtpServer, _emailSendingOptions.Port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_emailSendingOptions.Login, _emailSendingOptions.Password)
            };
        }
    }
}
