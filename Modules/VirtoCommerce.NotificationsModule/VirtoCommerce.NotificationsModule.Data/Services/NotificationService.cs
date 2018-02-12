using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Services
{

    public class NotificationService : INotificationService
    {
        #region FakeNotifications

        public static GenericSearchResult<Notification> _result = new GenericSearchResult<Notification>()
        {
            TotalCount = 2,
            Results = new List<Notification>()
            {
                new EmailNotification()
                {
                    Id = "1",
                    Type = "RegistrationEmailNotification",
                    IsActive = true,
                    CC = new string[] {},
                    BCC = new string[] {},
                    From = "a@a.com",
                    To = "s@s.s",
                    Templates = new List<NotificationTemplate>()
                    {
                        new EmailNotificationTemplate
                        {
                            Id = "1",
                            LanguageCode = "en-US",
                            Subject = "some",
                            Body = "Thank you for registration {{firstname}} {{lastname}}",
                        }
                    }
                },
                new SmsNotification()
                {
                    Id = "2",
                    TenantType = "Notifications",
                    Type = "TwoFactorEmailNotification",
                    IsActive = true,
                }
            }
        };

        #endregion

        public Notification GetNotificationByType(string type, string tenantId = null)
        {
            return _result.Results.Single(n => n.Type.Equals(type));
        }

        public Notification[] GetNotificationsByIds(string ids)
        {
            var arrayIds = ids.Split(';');
            return _result.Results.Where(r => arrayIds.Any(a => a == r.Id)).ToArray();
        }

        public void SaveChanges(Notification[] notifications)
        {
            //var found = _result.Results.Single(n => n.Id.Equals(notification.Id));
            //found.IsActive = notification.IsActive;
            throw new System.NotImplementedException();
        }

        

        
    }
}
