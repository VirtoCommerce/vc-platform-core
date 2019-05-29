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
using Assert = Xunit.Assert;

namespace VirtoCommerce.NotificationsModule.Tests.UnitTests
{
    public class NotificationServiceUnitTests
    {
        private readonly Mock<INotificationRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly INotificationRegistrar _notificationRegistrar;
        private readonly Func<INotificationRepository> _repositoryFactory;
        private readonly Mock<IEventPublisher> _eventPublisherMock;
        private readonly NotificationService _notificationService;

        public NotificationServiceUnitTests()
        {
            _repositoryMock = new Mock<INotificationRepository>();
            _repositoryFactory = () => _repositoryMock.Object;
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _repositoryMock.Setup(ss => ss.UnitOfWork).Returns(_mockUnitOfWork.Object);
            _eventPublisherMock = new Mock<IEventPublisher>();
            _notificationService = new NotificationService(_repositoryFactory, _eventPublisherMock.Object);
            _notificationRegistrar = _notificationService;

            if (!AbstractTypeFactory<NotificationEntity>.AllTypeInfos.SelectMany(x => x.AllSubclasses).Contains(typeof(EmailNotificationEntity)))
                AbstractTypeFactory<NotificationEntity>.RegisterType<EmailNotificationEntity>();
        }

        [Fact]
        public async Task GetNotificationsByIdsAsync_ReturnNotifications()
        {
            //Arrange
            var id = Guid.NewGuid().ToString();
            var responseGroup = NotificationResponseGroup.Default.ToString();
            var notifications = new List<NotificationEntity> { new EmailNotificationEntity() { Id = id, Type = nameof(EmailNotification) } };
            _repositoryMock.Setup(n => n.GetByIdsAsync(new[] { id }, responseGroup))
                .ReturnsAsync(notifications.ToArray());
            _notificationRegistrar.RegisterNotification<RegistrationEmailNotification>();

            //Act
            var result = await _notificationService.GetByIdsAsync(new[] { id }, responseGroup);

            //Assert
            Assert.NotNull(result);
            Assert.Contains(result, r => r.Id.Equals(id));
        }

        [Fact]
        public async Task SaveChangesAsync_SavedNotification()
        {
            //Arrange
            var id = Guid.NewGuid().ToString();
            var notificationEntities = new List<NotificationEntity>
            {
                new EmailNotificationEntity()
                {
                    Id = id,
                    Type = nameof(EmailNotification),
                    Kind = nameof(EmailNotification)
                }
            };
            var responseGroup = NotificationResponseGroup.Default.ToString();
            _repositoryMock.Setup(n => n.GetByIdsAsync(new[] { id }, responseGroup))
                .ReturnsAsync(notificationEntities.ToArray());
            var notifications = notificationEntities.Select(n => n.ToModel(AbstractTypeFactory<Notification>.TryCreateInstance(n.Type)));

            //Act
            await _notificationService.SaveChangesAsync(notifications.ToArray());
        }
    }
}
