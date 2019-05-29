using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.NotificationsModule.Data.Services;
using VirtoCommerce.NotificationsModule.Tests.NotificationTypes;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;
using Xunit;

namespace VirtoCommerce.NotificationsModule.Tests.UnitTests
{
    public class NotificationMessageServiceUnitTests
    {
        private readonly Mock<INotificationRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IEventPublisher> _eventPublisherMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly NotificationMessageService _notificationMessageService;

        public NotificationMessageServiceUnitTests()
        {
            _repositoryMock = new Mock<INotificationRepository>();
            Func<INotificationRepository> factory = () => _repositoryMock.Object;
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _repositoryMock.Setup(ss => ss.UnitOfWork).Returns(_mockUnitOfWork.Object);
            _eventPublisherMock = new Mock<IEventPublisher>();
            _notificationServiceMock = new Mock<INotificationService>();
            _notificationMessageService = new NotificationMessageService(factory, _eventPublisherMock.Object, _notificationServiceMock.Object);

            if (!AbstractTypeFactory<NotificationMessageEntity>.AllTypeInfos.SelectMany(x => x.AllSubclasses).Contains(typeof(EmailNotificationMessageEntity)))
                AbstractTypeFactory<NotificationMessageEntity>.RegisterType<EmailNotificationMessageEntity>();

            //if (!AbstractTypeFactory<Notification>.AllTypeInfos.SelectMany(x => x.AllSubclasses).Contains(typeof(EmailNotification)))
            //    AbstractTypeFactory<Notification>.RegisterType<EmailNotification>().MapToType<NotificationEntity>();

            if (!AbstractTypeFactory<NotificationTemplate>.AllTypeInfos.SelectMany(x => x.AllSubclasses).Contains(typeof(EmailNotificationTemplate)))
                AbstractTypeFactory<NotificationTemplate>.RegisterType<EmailNotificationTemplate>().MapToType<NotificationTemplateEntity>();

            if (!AbstractTypeFactory<NotificationMessage>.AllTypeInfos.SelectMany(x => x.AllSubclasses).Contains(typeof(EmailNotificationMessage)))
                AbstractTypeFactory<NotificationMessage>.RegisterType<EmailNotificationMessage>().MapToType<NotificationMessageEntity>();

            var notificationService = new NotificationService(null, null);
            notificationService.RegisterNotification<RegistrationEmailNotification>();
        }

        [Fact]
        public async Task GetNotificationsMessageByIds_GetMessage()
        {
            //Arrange
            var id = Guid.NewGuid().ToString();
            var notificationId = Guid.NewGuid().ToString();
            var message = new EmailNotificationMessageEntity { Id = id, NotificationType = nameof(EmailNotificationMessage), NotificationId = notificationId };
            var messages = new List<NotificationMessageEntity> { message };
            _repositoryMock.Setup(n => n.GetMessagesByIdsAsync(new[] { id })).ReturnsAsync(messages.ToArray());
            var notification = AbstractTypeFactory<Notification>.TryCreateInstance(nameof(RegistrationEmailNotification));
            notification.Id = notificationId;
            _notificationServiceMock.Setup(n => n.GetByIdsAsync(new[] { notificationId }, null)).ReturnsAsync(new[] { notification });

            //Act
            var result = await _notificationMessageService.GetNotificationsMessageByIds(new[] { id });

            //Assert
            Assert.NotNull(result);
            Assert.Contains(result, r => r.Id.Equals(id));
        }

        [Fact]
        public async Task SaveNotificationMessages_SaveMessage()
        {
            //Arrange
            var id = Guid.NewGuid().ToString();
            var messages = new List<EmailNotificationMessage>
            {
                new EmailNotificationMessage()
                {
                    Id = id,
                    NotificationId = Guid.NewGuid().ToString(),
                    NotificationType = nameof(RegistrationEmailNotification)
                }
            };
            var messageEntity = new EmailNotificationMessageEntity() { Id = id, NotificationType = nameof(EmailNotificationMessage) };
            var messageEntities = new List<NotificationMessageEntity> { messageEntity };
            _repositoryMock.Setup(n => n.GetMessagesByIdsAsync(new[] { id })).ReturnsAsync(messageEntities.ToArray());

            //Act
            await _notificationMessageService.SaveNotificationMessagesAsync(messages.ToArray());
        }
    }
}
