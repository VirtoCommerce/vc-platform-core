using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.NotificationsModule.Data.Abstractions;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.NotificationsModule.Data.Services
{

    public class NotificationServiceImpl : INotificationService
    {
        #region FakeNotifications

        static GenericSearchResult<NotificationResult> _result = new GenericSearchResult<NotificationResult>()
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
                    MaxAttemptCount = 10,
                    Templates = new List<NotificationTemplateResult>()
                    {
                        new NotificationTemplateResult
                        {
                            Id = "1",
                            NotificationType = "RegistrationEmailNotification",
                            Language = "en-US",
                            IsDefault = false,
                            Created = "2018-01-01",
                            Modified = "2018-01-01",
                            SendGatewayType = "Email",
                            CcRecipients = new string[] {},
                            BccRecipients = new string[] {},
                            Recipient = "a@a.com",
                            Sender = "s@s.s",
                            Subject = "some",
                            Body = "Thank you for registration {{firstname}} {{lastname}}",
                            DynamicProperties =  "{\n \"firstname\": \"Name\",\n \"lastname\": \"Last\"\n}"
                        }
                    }
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

        public void UpdateNotification(Notification notification)
        {
            var found = _result.Results.Single(n => n.Id.Equals(notification.Id));
            found.IsActive = notification.IsActive;
            //throw new System.NotImplementedException();
        }

        public void DeleteNotification(string id)
        {
            throw new System.NotImplementedException();
        }

        public GenericSearchResult<NotificationResult> SearchNotifications(NotificationSearchCriteria criteria)
        {
            var query = _result.Results.AsQueryable();

            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                query = query.Where(n => n.NotificationType.Contains(criteria.Keyword));
            }

            var totalCount = query.Count();

            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<NotificationResult>(x => x.NotificationType), SortDirection = SortDirection.Ascending } };
            }
            
            var collection = query.OrderBySortInfos(sortInfos).Skip(criteria.Skip).Take(criteria.Take).ToList();
            
            return new GenericSearchResult<NotificationResult>
            {
                Results = collection,
                TotalCount = totalCount
            };
        }
    }
}
