using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using VirtoCommerce.NotificationsModule.Core.Events;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.NotificationsModule.Data.Validation;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.NotificationsModule.Data.Services
{
    public class NotificationMessageService : INotificationMessageService
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
                var messages = await repository.GetMessageByIdAsync(ids);
                return messages.Select(n => n.ToModel(AbstractTypeFactory<NotificationMessage>.TryCreateInstance())).ToArray();
            }
        }

        public async Task SaveNotificationMessagesAsync(NotificationMessage[] messages)
        {
            ValidateMessageProperties(messages);

            var changedEntries = new List<GenericChangedEntry<NotificationMessage>>();

            using (var repository = _repositoryFactory())
            {
                var existingMessageEntities = await repository.GetMessageByIdAsync(messages.Select(m => m.Id).ToArray());
                foreach (var message in messages)
                {
                    var originalEntity = existingMessageEntities.FirstOrDefault(n => n.Id.Equals(message.Id));
                    var modifiedEntity = AbstractTypeFactory<NotificationMessageEntity>.TryCreateInstance().FromModel(message);

                    if (originalEntity != null)
                    {
                        changedEntries.Add(new GenericChangedEntry<NotificationMessage>(message, originalEntity.ToModel(AbstractTypeFactory<NotificationMessage>.TryCreateInstance()), EntryState.Modified));
                        modifiedEntity?.Patch(originalEntity);
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                        changedEntries.Add(new GenericChangedEntry<NotificationMessage>(message, EntryState.Added));
                    }
                }

                await _eventPublisher.Publish(new NotificationMessageChangingEvent(changedEntries));
                await repository.UnitOfWork.CommitAsync();
                await _eventPublisher.Publish(new NotificationMessageChangedEvent(changedEntries));
            }
        }

        private void ValidateMessageProperties(IEnumerable<NotificationMessage> messages)
        {
            if (messages == null)
            {
                throw new ArgumentNullException(nameof(messages));
            }

            var validator = new NotificationMessageValidator();
            foreach (var notification in messages)
            {
                validator.ValidateAndThrow(notification);
            }
        }
    }
}
