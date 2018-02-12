using System.Linq;
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

        public GenericSearchResult<Notification> SearchNotifications(NotificationSearchCriteria criteria)
        {
            var query = _notificationRepository.Notifications;

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
            var list = collection.Select(c => c.ToModel(AbstractTypeFactory<Notification>.TryCreateInstance())).ToList();

            return new GenericSearchResult<Notification>
            {
                Results = list,
                TotalCount = totalCount
            };
        }
    }
}
