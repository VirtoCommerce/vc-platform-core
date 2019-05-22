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
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.NotificationsModule.Data.Services
{
    public class NotificationMessageService : INotificationMessageService
    {
        private readonly Func<INotificationRepository> _repositoryFactory;
        private readonly IEventPublisher _eventPublisher;
        private readonly INotificationService _notificationService;

        public NotificationMessageService(Func<INotificationRepository> repositoryFactory, IEventPublisher eventPublisher, INotificationService notificationService)
        {
            _repositoryFactory = repositoryFactory;
            _eventPublisher = eventPublisher;
            _notificationService = notificationService;
        }

        public async Task<NotificationMessage[]> GetNotificationsMessageByIds(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                var messages = await repository.GetMessagesByIdsAsync(ids);
                return messages.Select(n =>
                {
                    var notification = _notificationService.GetByIdsAsync(new[] { n.NotificationId }).GetAwaiter().GetResult().FirstOrDefault();
                    return n.ToModel(AbstractTypeFactory<NotificationMessage>.TryCreateInstance($"{notification?.Kind}Message"));
                }).ToArray();
            }
        }

        public async Task SaveNotificationMessagesAsync(NotificationMessage[] messages)
        {
            ValidateMessageProperties(messages);

            var changedEntries = new List<GenericChangedEntry<NotificationMessage>>();
            var pkMap = new PrimaryKeyResolvingMap();

            using (var repository = _repositoryFactory())
            {
                var existingMessageEntities = await repository.GetMessagesByIdsAsync(messages.Select(m => m.Id).ToArray());
                foreach (var message in messages)
                {
                    var originalEntity = existingMessageEntities.FirstOrDefault(n => n.Id.Equals(message.Id));
                    var notification = (await _notificationService.GetByIdsAsync(new[] { message.NotificationId })).FirstOrDefault();
                    var modifiedEntity = AbstractTypeFactory<NotificationMessageEntity>.TryCreateInstance($"{notification?.Kind}MessageEntity").FromModel(message, pkMap);

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
                pkMap.ResolvePrimaryKeys();
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
