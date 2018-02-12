using System.Linq;
using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Services
{
    public class NotificationSearchService : INotificationSearchService
    {
        public GenericSearchResult<Notification> SearchNotifications(NotificationSearchCriteria criteria)
        {
            //TODO
            var query = NotificationService._result.Results.AsQueryable();

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
