using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.NotificationsModule.Data.Services
{

    public class NotificationService : ServiceBase, INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly Func<INotificationRepository> _repositoryFactory;

        public NotificationService(INotificationRepository notificationRepository, Func<INotificationRepository> repositoryFactory)
        {
            _notificationRepository = notificationRepository;
            _repositoryFactory = repositoryFactory;
        }

        public async Task<Notification> GetNotificationByTypeAsync(string type, string tenantId = null)
        {
            var notification = await _notificationRepository.GetNotificationEntityByTypeAsync(type, tenantId, null);

            return notification.ToModel(AbstractTypeFactory<Notification>.TryCreateInstance(notification.Kind));
        }

        public async Task<Notification[]> GetNotificationsByIdsAsync(string ids)
        {
            var notifications = await _notificationRepository.GetNotificationByIdsAsync(ids.Split(';'));
            return notifications.Select(n => n.ToModel(AbstractTypeFactory<Notification>.TryCreateInstance(n.Kind))).ToArray();
        }

        public async Task SaveChangesAsync(Notification[] notifications)
        {
            if (notifications != null && notifications.Any())
            {
                var pkMap = new PrimaryKeyResolvingMap();
                using (var repository = _repositoryFactory())
                using (var changeTracker = new ObservableChangeTracker())
                {
                    var existingNotificationEntities = await repository.GetNotificationByIdsAsync(notifications.Select(m => m.Id).ToArray());
                    foreach (var notification in notifications)
                    {
                        var dataTargetNotification = existingNotificationEntities.FirstOrDefault(n => n.Id.Equals(notification.Id));
                        var modifiedEntity = AbstractTypeFactory<NotificationEntity>.TryCreateInstance().FromModel(notification, pkMap);

                        if (dataTargetNotification != null)
                        {
                            changeTracker.Attach(dataTargetNotification);
                            modifiedEntity?.Patch(dataTargetNotification);
                        }
                        else
                        {
                            repository.Add(modifiedEntity);
                        }
                    }

                    CommitChanges(repository);
                    pkMap.ResolvePrimaryKeys();
                }
            }
        }

        

        
    }
}
