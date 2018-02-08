using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Services
{

    public class NotificationServiceImpl : INotificationService
    {
        #region FakeNotifications

        static GenericSearchResult<Notification> _result = new GenericSearchResult<Notification>()
        {
            TotalCount = 2,
            Results = new List<Notification>()
            {
                new EmailNotification()
                {
                    Id = "1",
                    TenantId = "notifications.types.RegistrationEmailNotification.displayName",
                    //Description = "notifications.types.RegistrationEmailNotification.description",
                    TenantType = "Email",
                    Type = "RegistrationEmailNotification",
                    IsActive = true,
                    CC = new string[] {},
                    BCC = new string[] {},
                    From = "a@a.com",
                    To = "s@s.s",
                    //IsSuccessSend = false,
                    //AttemptCount = 0,
                    //MaxAttemptCount = 10,
                    Templates = new List<NotificationTemplate>()
                    {
                        new EmailNotificationTemplate
                        {
                            Id = "1",
                            LanguageCode = "en-US",
                            
                            Subject = "some",
                            Body = "Thank you for registration {{firstname}} {{lastname}}",
                            //DynamicProperties =  "{\n \"firstname\": \"Name\",\n \"lastname\": \"Last\"\n}"
                        }
                    }
                },
                new SmsNotification()
                {
                    Id = "2",
                    TenantId = "notifications.types.TwoFactorEmailNotification.displayName",
                    //Description = "notifications.types.TwoFactorEmailNotification.description",
                    TenantType = "SMS",
                    Type = "TwoFactorEmailNotification",
                    IsActive = true,
                    //IsSuccessSend = false,
                    //AttemptCount = 0,
                    //MaxAttemptCount = 10
                }
            }
        };

        #endregion
        
        public GenericSearchResult<Notification> GetNotifications()
        {
            return _result;
        }

        public Notification GetNotificationByTypeId(string typeId)
        {
            return _result.Results.Single(n => n.Type.Equals(typeId));
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

        public GenericSearchResult<Notification> SearchNotifications(NotificationSearchCriteria criteria)
        {
            var query = _result.Results.AsQueryable();

            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                query = query.Where(n => n.Type.Contains(criteria.Keyword));
            }

            var totalCount = query.Count();

            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<Notification>(x => x.Type), SortDirection = SortDirection.Ascending } };
            }
            
            var collection = query.OrderBySortInfos(sortInfos).Skip(criteria.Skip).Take(criteria.Take).ToList();
            
            return new GenericSearchResult<Notification>
            {
                Results = collection,
                TotalCount = totalCount
            };
        }
    }
}
