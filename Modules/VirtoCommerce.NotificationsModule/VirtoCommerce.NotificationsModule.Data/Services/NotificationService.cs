using System;
using System.Collections.Generic;
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

    public class NotificationService : ServiceBase, INotificationService, INotificationRegistrar
    {
        private readonly Func<INotificationRepository> _repositoryFactory;

        public NotificationService(Func<INotificationRepository> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task<Notification> GetNotificationByTypeAsync(string type, string tenantId = null, string tenantType = null)
        {
            var notificationType = AbstractTypeFactory<Notification>.AllTypeInfos.FirstOrDefault(t => t.Type.Name.Equals(type))?.Type;
            if (notificationType == null) return null;

            var result = AbstractTypeFactory<Notification>.TryCreateInstance(notificationType.Name);
            using (var repository = _repositoryFactory())
            {
                var notification = await repository.GetNotificationEntityByTypeAsync(notificationType.Name, tenantId, tenantType);
                if (notification != null)
                    return notification.ToModel(result);
            }
            return result;
        }

        public async Task<Notification[]> GetNotificationsByIdsAsync(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                var notifications = await repository.GetNotificationByIdsAsync(ids);
                return notifications.Select(n => n.ToModel(AbstractTypeFactory<Notification>.TryCreateInstance(n.Type))).ToArray();
            }
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


        public void RegisterNotification<T>() where T : Notification
        {
            if (AbstractTypeFactory<Notification>.AllTypeInfos.All(t => t.Type != typeof(T)))
            {
                AbstractTypeFactory<Notification>.RegisterType<T>();
            }
        }
    }
}
