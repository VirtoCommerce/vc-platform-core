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
    public class NotificationMessageService : ServiceBase, INotificationMessageService
    {
        private readonly Func<INotificationRepository> _repositoryFactory;

        public NotificationMessageService(Func<INotificationRepository> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task<NotificationMessage[]> GetNotificationsMessageByIds(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                var messages = await repository.GetNotificationMessageByIdAsync(ids);
                return messages.Select(n => n.ToModel(AbstractTypeFactory<NotificationMessage>.TryCreateInstance())).ToArray();

            }
        }

        public async Task SaveNotificationMessages(NotificationMessage[] messages)
        {
            using (var repository = _repositoryFactory())
            using (var changeTracker = new ObservableChangeTracker())
            {
                var existingMessageEntities = await repository.GetNotificationMessageByIdAsync(messages.Select(m => m.Id).ToArray());
                foreach (var message in messages)
                {
                    var dataTargetNotification = existingMessageEntities.FirstOrDefault(n => n.Id.Equals(message.Id));
                    var modifiedEntity = AbstractTypeFactory<NotificationMessageEntity>.TryCreateInstance().FromModel(message);

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
            }
        }
    }
}
