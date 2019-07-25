using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
        private readonly INotificationService _notificationService;
        public NotificationSearchService(Func<INotificationRepository> repositoryFactory, INotificationService notificationService)
        {
            _repositoryFactory = repositoryFactory;
            _notificationService = notificationService;
        }

        public async Task<NotificationSearchResult> SearchNotificationsAsync(NotificationSearchCriteria criteria)
        {
            var result = AbstractTypeFactory<NotificationSearchResult>.TryCreateInstance();

            var sortInfos = BuildSortExpression(criteria);
            var tmpSkip = 0;
            var tmpTake = 0;

            using (var repository = _repositoryFactory())
            {
                var query = BuildQuery(repository, criteria, sortInfos);

                result.TotalCount = await query.CountAsync();
                if (criteria.Take > 0)
                {
                    var notificationIds = await query.OrderBySortInfos(sortInfos).ThenBy(x=>x.Id)
                                                     .Select(x => x.Id)
                                                     .Skip(criteria.Skip).Take(criteria.Take)
                                                     .ToArrayAsync();
                    var unorderedResults = await _notificationService.GetByIdsAsync(notificationIds);
                    result.Results = unorderedResults.OrderBy(x => Array.IndexOf(notificationIds, x.Id)).ToList();
                }
            }
            tmpSkip = Math.Min(result.TotalCount, criteria.Skip);
            tmpTake = Math.Min(criteria.Take, Math.Max(0, result.TotalCount - criteria.Skip));

            criteria.Skip -= tmpSkip;
            criteria.Take -= tmpTake;

            if (criteria.Take > 0)
            {
                var transientNotificationsQuery = AbstractTypeFactory<Notification>.AllTypeInfos.Select(x => AbstractTypeFactory<Notification>.TryCreateInstance(x.Type.Name))
                                                                              .OfType<Notification>().AsQueryable();
                if (!string.IsNullOrEmpty(criteria.NotificationType))
                {
                    transientNotificationsQuery = transientNotificationsQuery.Where(x => x.Type.EqualsInvariant(criteria.NotificationType));
                }

                var allPersistentProvidersTypes = result.Results.Select(x => x.GetType()).Distinct();
                transientNotificationsQuery = transientNotificationsQuery.Where(x => !allPersistentProvidersTypes.Contains(x.GetType()));

                transientNotificationsQuery = transientNotificationsQuery.Where(x => !x.Kind.EqualsInvariant(x.Type));

                result.TotalCount += transientNotificationsQuery.Count();
                var transientNotifications = transientNotificationsQuery.Skip(criteria.Skip).Take(criteria.Take).ToList();

                result.Results = result.Results.Concat(transientNotifications).AsQueryable().OrderBySortInfos(sortInfos).ToList();
            }
            return result;
        }

        protected virtual IQueryable<NotificationEntity> BuildQuery(INotificationRepository repository, NotificationSearchCriteria criteria, IEnumerable<SortInfo> sortInfos)
        {
            var query = repository.Notifications;

            if (!string.IsNullOrEmpty(criteria.NotificationType))
            {
                query = query.Where(x => x.Type == criteria.NotificationType);
            }

            if (!string.IsNullOrEmpty(criteria.TenantId))
            {
                query = query.Where(x => x.TenantId == criteria.TenantId);
            }

            if (!string.IsNullOrEmpty(criteria.TenantType))
            {
                query = query.Where(x => x.TenantType == criteria.TenantType);
            }

            query = query.OrderBySortInfos(sortInfos);
            return query;
        }

        protected virtual IList<SortInfo> BuildSortExpression(NotificationSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo
                    {
                        SortColumn = nameof(NotificationEntity.Type)
                    }
                };
            }
            return sortInfos;
        }
    }
}
