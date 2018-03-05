using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Services
{
    public class NotificationSearchService : INotificationSearchService
    {
        private readonly INotificationRepository _notificationRepository;
        
        public NotificationSearchService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public  GenericSearchResult<Notification> SearchNotifications(NotificationSearchCriteria criteria)
        {
            var query = AbstractTypeFactory<Notification>.AllTypeInfos
                .Where(t => t.Type.IsSubclassOf(typeof(EmailNotification)) || t.Type.IsSubclassOf(typeof(SmsNotification)))
                .Select(n => n.Type)
                .AsQueryable();
            
            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                query = query.Where(n => n.Name.Contains(criteria.Keyword));
            }

            var totalCount = query.Count();

            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<Notification>(x => x.Type), SortDirection = SortDirection.Ascending } };
            }

            var collection = query.OrderBySortInfos(sortInfos).Skip(criteria.Skip).Take(criteria.Take).ToList();

            var list = collection.Select(t =>
            {
                var result = AbstractTypeFactory<Notification>.TryCreateInstance(t.Name);
                var notificationEntity = _notificationRepository.Notifications.FirstOrDefault(ne => ne.Type.Equals(t.Name));
                return notificationEntity != null ? notificationEntity.ToModel(result) : result;
            }).ToList();

            return new GenericSearchResult<Notification>
            {
                Results = list,
                TotalCount = totalCount
            };
        }
    }
}
