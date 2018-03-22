using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.NotificationsModule.Core.Events;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.NotificationsModule.Data.Services
{
    public class NotificationMessageService : ServiceBase, INotificationMessageService
    {
        private readonly Func<INotificationRepository> _repositoryFactory;
        private readonly IEventPublisher _eventPublisher;

        public NotificationMessageService(Func<INotificationRepository> repositoryFactory, IEventPublisher eventPublisher)
        {
            _repositoryFactory = repositoryFactory;
            _eventPublisher = eventPublisher;
        }

        public async Task<NotificationMessage[]> GetNotificationsMessageByIds(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                var messages = await repository.GetNotificationMessageByIdAsync(ids);
                return messages.Select(n => n.ToModel(AbstractTypeFactory<NotificationMessage>.TryCreateInstance())).ToArray();
            }
        }

        public async Task SaveNotificationMessagesAsync(NotificationMessage[] messages)
        {
            var changedEntries = new List<ChangedEntry<NotificationMessage>>();

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
                        changedEntries.Add(new ChangedEntry<NotificationMessage>(message, EntryState.Modified));
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                        changedEntries.Add(new ChangedEntry<NotificationMessage>(message, EntryState.Added));
                    }
                }

                await _eventPublisher.Publish(new GenericNotificationMessageChangingEvent<NotificationMessage>(changedEntries));
                CommitChanges(repository);
                await _eventPublisher.Publish(new GenericNotificationMessageChangedEvent<NotificationMessage>(changedEntries));
            }
        }
    }
}
