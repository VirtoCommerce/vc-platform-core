using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.NotificationsModule.Data.Services
{

    public class NotificationService : INotificationService
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
                using (var repository = _repositoryFactory())
                using (var changeTracker = new ObservableChangeTracker())
                {
                    var existingNotificationEntities = await repository.GetNotificationByIdsAsync(notifications.Select(m => m.Id).ToArray());
                    foreach (var notification in notifications)
                    {
                        var dataTargetNotification = existingNotificationEntities.FirstOrDefault(n => n.Id.Equals(notification.Id));

                        if (dataTargetNotification != null)
                        {
                            dataTargetNotification.FromModel(notification);
                            changeTracker.Attach(dataTargetNotification);
                        }
                        else
                        {
                            var notificationEntity = AbstractTypeFactory<NotificationEntity>.TryCreateInstance();
                            if (notificationEntity != null)
                            {
                                notificationEntity = notificationEntity.FromModel(notification);
                                repository.Add(notificationEntity);
                            }
                        }
                    }

                    repository.UnitOfWork.Commit();
                }
            }
        }

        

        
    }
}
