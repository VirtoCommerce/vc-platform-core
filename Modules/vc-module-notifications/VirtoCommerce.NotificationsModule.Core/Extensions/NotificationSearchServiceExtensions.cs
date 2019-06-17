using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Core.Extensions
{
    public static class NotificationSearchServiceExtensions
    {
        public static async Task<T> GetNotificationAsync<T>(this INotificationSearchService service, TenantIdentity tenant = null) where T : Notification
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }
            var result = await service.GetNotificationAsync(typeof(T).Name, tenant);
            return result as T;
        }

        public static async Task<Notification> GetNotificationAsync(this INotificationSearchService service, string notificationType, TenantIdentity tenant = null)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }
            var criteria = AbstractTypeFactory<NotificationSearchCriteria>.TryCreateInstance();
            criteria.NotificationType = notificationType;
            criteria.Take = int.MaxValue;
            var searchResult = await service.SearchNotificationsAsync(criteria);
            //Find first global notification (without tenant)
            var result = searchResult.Results.Where(x => x.TenantIdentity.IsEmpty).FirstOrDefault();
            if (tenant != null)
            {
                //If tenant is specified try to find a notification belongs to concrete tenant or use default as fallback
                result = searchResult.Results.Where(x => x.TenantIdentity == tenant).FirstOrDefault() ?? result;
            }
            return result;
        }
    }
}
