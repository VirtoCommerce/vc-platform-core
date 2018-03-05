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
using VirtoCommerce.NotificationsModule.Tests.NotificationTypes;
using VirtoCommerce.Platform.Core.Common;
using Xunit;

namespace VirtoCommerce.NotificationsModule.Tests.UnitTests
{
    public class NotificationServiceUnitTests
    {
        private readonly Mock<INotificationRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly INotificationRegistrar _notificationRegistrar;
        private readonly Func<INotificationRepository> _repositoryFactory;
        private readonly NotificationService _notificationService;

        public NotificationServiceUnitTests()
        {
            _repositoryMock = new Mock<INotificationRepository>();
            _repositoryFactory = () => _repositoryMock.Object;
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _repositoryMock.Setup(ss => ss.UnitOfWork).Returns(_mockUnitOfWork.Object);
            _notificationService = new NotificationService(_repositoryFactory);
            _notificationRegistrar = _notificationService;
        }

        [Fact]
        public async Task GetNotificationByTypeAsync_ReturnNotifiction()
        {
            //Arrange
            string type = nameof(RegistrationEmailNotification);
            _repositoryMock.Setup(n => n.GetNotificationEntityForListByType(nameof(RegistrationEmailNotification), null, null))
                .Returns(new Data.Model.NotificationEntity() { IsActive = true });
            _notificationRegistrar.RegisterNotification<RegistrationEmailNotification>();

            //Act
            var result = await _notificationService.GetNotificationByTypeAsync(type, null, null);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(type, result.Type);
        }

        [Fact]
        public async Task GetNotificationByTypeAsync_ReturnNull()
        {
            //Arrange
            string type = nameof(RegistrationEmailNotification);
            _repositoryMock.Setup(n => n.GetNotificationEntityForListByType(nameof(RegistrationEmailNotification), null, null))
                .Returns(new NotificationEntity() { IsActive = true, Type = type });

            //Act
            var result = await _notificationService.GetNotificationByTypeAsync(type, null, null);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetNotificationsByIdsAsync_ReturnNotifications()
        {
            //Arrange
            string id = Guid.NewGuid().ToString();
            AbstractTypeFactory<Notification>.RegisterType<EmailNotification>().MapToType<NotificationEntity>();
            var notifications = new List<NotificationEntity> { new NotificationEntity(){ Id = id, Type = nameof(EmailNotification)}};
            _repositoryMock.Setup(n => n.GetNotificationByIdsAsync(new[] { id }))
                .ReturnsAsync(notifications.ToArray());
            _notificationRegistrar.RegisterNotification<RegistrationEmailNotification>();

            //Act
            var result = await _notificationService.GetNotificationsByIdsAsync(new [] { id });

            //Assert
            Assert.NotNull(result);
            Assert.Contains(result, r => r.Id.Equals(id));
        }

        [Fact]
        public async Task SaveChangesAsync_SavedNotification()
        {
            //Arrange
            string id = Guid.NewGuid().ToString();
            AbstractTypeFactory<Notification>.RegisterType<EmailNotification>().MapToType<NotificationEntity>();
            var notificationEntities = new List<NotificationEntity> { new NotificationEntity() { Id = id, Type = nameof(EmailNotification) } };
            _repositoryMock.Setup(n => n.GetNotificationByIdsAsync(new[] { id }))
                .ReturnsAsync(notificationEntities.ToArray());
            var notifications = notificationEntities.Select(n => n.ToModel(AbstractTypeFactory<Notification>.TryCreateInstance(n.Type)));

            //Act
            await _notificationService.SaveChangesAsync(notifications.ToArray());
        }
    }
}
