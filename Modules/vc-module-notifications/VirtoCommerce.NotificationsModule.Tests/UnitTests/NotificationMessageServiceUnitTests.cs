using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.NotificationsModule.Data.Services;
using VirtoCommerce.Platform.Core.Common;
using Xunit;

namespace VirtoCommerce.NotificationsModule.Tests.UnitTests
{
    public class NotificationMessageServiceUnitTests
    {
        private readonly Mock<INotificationRepository> _repositoryMock;
        private readonly INotificationRegistrar _notificationRegistrar;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly NotificationMessageService _notificationMessageService;

        public NotificationMessageServiceUnitTests()
        {
            _repositoryMock = new Mock<INotificationRepository>();
            Func<INotificationRepository> factory = () => _repositoryMock.Object;
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _repositoryMock.Setup(ss => ss.UnitOfWork).Returns(_mockUnitOfWork.Object);
            _notificationMessageService = new NotificationMessageService(factory);
            //todo
            if (!AbstractTypeFactory<Notification>.AllTypeInfos.Any(t => t.IsAssignableTo(nameof(EmailNotification))))
            {
                AbstractTypeFactory<Notification>.RegisterType<EmailNotification>().MapToType<NotificationEntity>();
            }

            if (!AbstractTypeFactory<NotificationTemplate>.AllTypeInfos.Any(t => t.IsAssignableTo(nameof(EmailNotificationTemplate))))
            {
                AbstractTypeFactory<NotificationTemplate>.RegisterType<EmailNotificationTemplate>().MapToType<NotificationTemplateEntity>();
            }

            if (!AbstractTypeFactory<NotificationMessage>.AllTypeInfos.Any(t => t.IsAssignableTo(nameof(EmailNotificationMessage))))
            {
                AbstractTypeFactory<NotificationMessage>.RegisterType<EmailNotificationMessage>().MapToType<NotificationMessageEntity>();
            }
        }

        [Fact]
        public async Task GetNotificationsMessageByIds_GetMessage()
        {
            //Arrange
            string id = Guid.NewGuid().ToString();
            var message = new NotificationMessageEntity { Id = id, NotificationType = nameof(EmailNotificationMessage)};
            var messages = new List<NotificationMessageEntity> {message};
            _repositoryMock.Setup(n => n.GetNotificationMessageByIdAsync(new []{ id })).ReturnsAsync(messages.ToArray());

            //Act
            var result = await _notificationMessageService.GetNotificationsMessageByIds(new [] { id });

            //Assert
            Assert.NotNull(result);
            Assert.Contains(result, r => r.Id.Equals(id));
        }

        [Fact]
        public async Task SaveNotificationMessages_SaveMessage()
        {
            //Arrange
            string id = Guid.NewGuid().ToString();
            var messages = new List<EmailNotificationMessage> { new EmailNotificationMessage() { Id = id } };
            var messageEntity = new NotificationMessageEntity() { Id = id, NotificationType = nameof(EmailNotificationMessage) };
            var messageEntities = new List<NotificationMessageEntity> { messageEntity };
            _repositoryMock.Setup(n => n.GetNotificationMessageByIdAsync(new[] { id })).ReturnsAsync(messageEntities.ToArray());

            //Act
            await _notificationMessageService.SaveNotificationMessagesAsync(messages.ToArray());
        }
    }
}
