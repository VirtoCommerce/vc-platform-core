using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Data.Abstractions;

namespace VirtoCommerce.NotificationsModule.Data.Services
{

    public class NotificationServiceImpl : INotificationService
    {
        #region FakeNotifications

        GenericSearchResult<NotificationResult> _result = new GenericSearchResult<NotificationResult>()
        {
            TotalCount = 2,
            Results = new List<NotificationResult>()
            {
                new NotificationResult()
                {
                    Id = "1",
                    DisplayName = "notifications.types.RegistrationEmailNotification.displayName",
                    Description = "notifications.types.RegistrationEmailNotification.description",
                    SendGatewayType = "Email",
                    NotificationType = "RegistrationEmailNotification",
                    IsActive = true,
                    IsSuccessSend = false,
                    AttemptCount = 0,
                    MaxAttemptCount = 10
                },
                new NotificationResult()
                {
                    Id = "2",
                    DisplayName = "notifications.types.TwoFactorEmailNotification.displayName",
                    Description = "notifications.types.TwoFactorEmailNotification.description",
                    SendGatewayType = "SMS",
                    NotificationType = "TwoFactorEmailNotification",
                    IsActive = true,
                    IsSuccessSend = false,
                    AttemptCount = 0,
                    MaxAttemptCount = 10
                }
            }
        };

        #endregion
        
        public GenericSearchResult<NotificationResult> GetNotifications()
        {
            return _result;
        }

        public NotificationResult GetNotificationByTypeId(string typeId)
        {
            return _result.Results.Single(n => n.NotificationType.Equals(typeId));
        }

        public void UpdateNotification(NotificationResult notification)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteNotification(string id)
        {
            throw new System.NotImplementedException();
        }
    }
}
