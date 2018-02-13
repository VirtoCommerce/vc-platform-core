using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Services
{

    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

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

        public async Task<Notification> GetNotificationByType(string type, string tenantId = null)
        {
            var notification = await _notificationRepository.GetNotificationEntityByTypeAsync(type, tenantId, null);

            return notification.ToModel(AbstractTypeFactory<Notification>.TryCreateInstance());
        }

        public async Task<Notification[]> GetNotificationsByIds(string ids)
        {
            var notifications = await _notificationRepository.GetNotificationByIdsAsync(ids.Split(';'));
            return notifications.Select(n => n.ToModel(AbstractTypeFactory<Notification>.TryCreateInstance())).ToArray();
        }

        public void SaveChanges(Notification[] notifications)
        {
            //var found = _result.Results.Single(n => n.Id.Equals(notification.Id));
            //found.IsActive = notification.IsActive;
            throw new System.NotImplementedException();
        }

        

        
    }
}
