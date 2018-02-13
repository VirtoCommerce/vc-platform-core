using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Services
{

    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<Notification> GetNotificationByType(string type, string tenantId = null)
        {
            var notification = await _notificationRepository.GetNotificationEntityByTypeAsync(type, tenantId, null);

            return notification.ToModel(AbstractTypeFactory<Notification>.TryCreateInstance(notification.Kind));
        }

        public async Task<Notification[]> GetNotificationsByIds(string ids)
        {
            var notifications = await _notificationRepository.GetNotificationByIdsAsync(ids.Split(';'));
            return notifications.Select(n => n.ToModel(AbstractTypeFactory<Notification>.TryCreateInstance(n.Kind))).ToArray();
        }

        public void SaveChanges(Notification[] notifications)
        {
            //var found = _result.Results.Single(n => n.Id.Equals(notification.Id));
            //found.IsActive = notification.IsActive;
            throw new System.NotImplementedException();
        }

        

        
    }
}
