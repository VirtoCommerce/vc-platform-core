using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Model.Search;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Services
{
    public class NotificationMessageSearchService : INotificationMessageSearchService
    {
        private readonly Func<INotificationRepository> _repositoryFactory;

        public NotificationMessageSearchService(Func<INotificationRepository> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task<NotificationMessageSearchResult> SearchMessageAsync(NotificationMessageSearchCriteria criteria)
        {
            var result = new NotificationMessageSearchResult();

            using (var repository = _repositoryFactory())
            {
                result.Results = new List<NotificationMessage>();

                var query = repository.NotifcationMessages;

                if (!criteria.ObjectIds.IsNullOrEmpty())
                {
                    query = query.Where(n => criteria.ObjectIds.Contains(n.TenantId));
                }

                if (!string.IsNullOrEmpty(criteria.ObjectType))
                {
                    query = query.Where(n => n.TenantType == criteria.ObjectType);
                }

                if (!criteria.ObjectTypes.IsNullOrEmpty())
                {
                    query = query.Where(n => criteria.ObjectTypes.Contains(n.TenantType));
                }

                result.TotalCount = await query.CountAsync();

                if (criteria.Take > 0)
                {
                    var sortInfos = criteria.SortInfos;
                    if (sortInfos.IsNullOrEmpty())
                    {
                        sortInfos = new[] { new SortInfo { SortColumn = "CreatedDate", SortDirection = SortDirection.Descending } };
                    }
                    query = query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id);

                    result.Results = query.Skip(criteria.Skip)
                        .Take(criteria.Take)
                        .ToArray()
                        .Select(nm => nm.ToModel(AbstractTypeFactory<NotificationMessage>.TryCreateInstance()))
                        .ToList();
                }
            }

            return result;
        }
    }
}
