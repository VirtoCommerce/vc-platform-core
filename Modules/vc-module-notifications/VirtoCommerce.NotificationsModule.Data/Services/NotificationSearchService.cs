using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Services
{
    public class NotificationSearchService : INotificationSearchService
    {
        private readonly Func<INotificationRepository> _repositoryFactory;
        public NotificationSearchService(Func<INotificationRepository> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task<NotificationSearchResult> SearchNotificationsAsync(NotificationSearchCriteria criteria)
        {
            var result = AbstractTypeFactory<NotificationSearchResult>.TryCreateInstance();
            var query = AbstractTypeFactory<Notification>.AllTypeInfos
                .Where(t => t.AllSubclasses.Any(s => s != t.Type && s.IsSubclassOf(typeof(Notification))))
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

            NotificationEntity[] entities;

            using (var repository = _repositoryFactory())
            {
                entities = await repository.GetByTypesAsync(collection.Select(c => c.Name).ToArray(), criteria.TenantId,
                    criteria.TenantType, criteria.ResponseGroup, criteria.IsActive);
            }

            var notifications = collection.Select(t =>
            {
                var notification = AbstractTypeFactory<Notification>.TryCreateInstance(t.Name);
                var notificationEntity = entities.FirstOrDefault(e => e.Type.EqualsInvariant(t.Name));
                return notificationEntity != null ? notificationEntity.ToModel(notification) : notification;
            });

            if (criteria.IsActive)
            {
                notifications = notifications.Where(n => n.IsActive);
            }

            result.Results = notifications.ToList();
            result.TotalCount = totalCount;

            return result;
        }
    }
}
