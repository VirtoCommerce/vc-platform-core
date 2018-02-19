using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VirtoCommerce.NotificationsModule.Data.Enums;
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
                        Recipients = new ObservableCollection<NotificationEmailRecipientEntity>()
                        {
                            new NotificationEmailRecipientEntity() { EmailAddress = "cc1@cc.com", RecipientType = NotificationRecipientType.Cc},
                            new NotificationEmailRecipientEntity() { EmailAddress = "cc2@cc.com", RecipientType = NotificationRecipientType.Cc },
                            new NotificationEmailRecipientEntity() { EmailAddress = "bcc1@cc.com", RecipientType = NotificationRecipientType.Bcc},
                            new NotificationEmailRecipientEntity() { EmailAddress = "bcc2@cc.com", RecipientType = NotificationRecipientType.Bcc}
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
