using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VirtoCommerce.NotificationsModule.Data.Model;

namespace VirtoCommerce.NotificationsModule.Data.Repositories
{
    public static class DbContextExtension
    {

        public static void EnsureSeeded(this NotificationDbContext context)
        {
            if (!context.Set<NotificationEntity>().Any())
            {
                var notifications = new List<NotificationEntity>()
                {
                    new NotificationEntity
                    {
                        Type = "RegistrationEmailNotification",
                        Kind = "EmailNotification",
                        IsActive = true,
                        CcRecipients = new ObservableCollection<NotificationEmailRecipientEntity>()
                        {
                            new NotificationEmailRecipientEntity() { EmailAddress = "cc1@cc.com" },
                            new NotificationEmailRecipientEntity() { EmailAddress = "cc2@cc.com" }
                        },
                        BccRecipients = new ObservableCollection<NotificationEmailRecipientEntity>()
                        {
                            new NotificationEmailRecipientEntity() { EmailAddress = "bcc1@cc.com" },
                            new NotificationEmailRecipientEntity() { EmailAddress = "bcc2@cc.com" }
                        },
                        Templates = new ObservableCollection<NotificationTemplateEntity>()
                        {
                            new NotificationTemplateEntity()
                            {
                                LanguageCode = "en-US",
                                Subject = "Your login - {{ login }}.",
                                Body = "Thank you for registration {{firstname}} {{lastname}}",
                            }
                        }
                    },
                    new NotificationEntity
                    {
                        Type = "TwoFactorSmsNotification",
                        Kind = "SmsNotification",
                        IsActive = true,
                        Templates = new ObservableCollection<NotificationTemplateEntity>()
                        {
                            new NotificationTemplateEntity()
                            {
                                LanguageCode = "en-US",
                                Subject = "Security Code",
                                Message = "Your security code is {{ token }}",
                            }
                        }
                    },
                };
                context.AddRange(notifications);
                context.SaveChanges();
            }

            
        }
    }
}
