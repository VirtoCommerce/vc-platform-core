using System;
using System.Collections.Generic;
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

        public async Task<GenericSearchResult<Notification>> SearchNotificationsAsync(NotificationSearchCriteria criteria)
        {
            var sortInfos = GetSearchNotificationsSortInfo(criteria);
            var query = GetSearchNotificationsQuery(criteria, sortInfos);

            var totalCount = query.Count();

            var collection = query.Skip(criteria.Skip).Take(criteria.Take).ToList();

            NotificationEntity[] entities;

            using (var repository = _repositoryFactory())
            {
                entities = await repository.GetByTypesAsync(collection.Select(c => c.Name).ToArray(), criteria.TenantId, criteria.TenantType, criteria.ResponseGroup);
            }

            var list = collection.Select(t =>
            {
                var result = AbstractTypeFactory<Notification>.TryCreateInstance(t.Name);
                var notificationEntity = entities.FirstOrDefault(e => e.Type.Equals(t.Name));
                return notificationEntity != null ? notificationEntity.ToModel(result) : result;
            }).ToList();

            return new GenericSearchResult<Notification>
            {
                Results = list,
                TotalCount = totalCount
            };
        }

        private IList<SortInfo> GetSearchNotificationsSortInfo(NotificationSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<Notification>(x => x.Type), SortDirection = SortDirection.Ascending } };
            }

            return sortInfos;
        }

        private IQueryable<Type> GetSearchNotificationsQuery(NotificationSearchCriteria criteria, IList<SortInfo> sortInfos)
        {
            var query = AbstractTypeFactory<Notification>.AllTypeInfos
                .Where(t => t.AllSubclasses.Any(s => s != t.Type && s.IsSubclassOf(typeof(Notification))))
                .Select(n => n.Type)
                .AsQueryable();

            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                query = query.Where(n => n.Name.Contains(criteria.Keyword));
            }

            query = query.OrderBySortInfos(sortInfos);
            return query;
        }
    }
}
