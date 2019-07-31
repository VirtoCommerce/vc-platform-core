using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.NotificationsModule.Core.Extensions;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Core.Types;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.NotificationsModule.Data.Services;
using VirtoCommerce.NotificationsModule.Tests.NotificationTypes;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using Xunit;

namespace VirtoCommerce.NotificationsModule.Tests.UnitTests
{
    public class NotificationSearchServiceUnitTests
    {
        private readonly Mock<INotificationRepository> _repositoryMock;
        private readonly Mock<Func<INotificationRepository>> _repositoryFactoryMock;
        private readonly Func<INotificationRepository> _repositoryFactory;
        private readonly INotificationRegistrar _notificationRegistrar;
        private readonly Mock<IEventPublisher> _eventPublisherMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly NotificationSearchService _notificationSearchService;

        public NotificationSearchServiceUnitTests()
        {
            _repositoryMock = new Mock<INotificationRepository>();
            _repositoryFactoryMock = new Mock<Func<INotificationRepository>>();
            _eventPublisherMock = new Mock<IEventPublisher>();
            _notificationRegistrar = new NotificationRegistrar();
            _repositoryFactory = () => _repositoryMock.Object;
            _notificationServiceMock = new Mock<INotificationService>();
            _notificationSearchService = new NotificationSearchService(_repositoryFactory, _notificationServiceMock.Object);
        }

        [Fact]
        public async Task GetNotificationByTypeAsync_ReturnNotification()
        {
            //Arrange
            var type = nameof(RegistrationEmailNotification);

            var mockNotifications = new Common.TestAsyncEnumerable<NotificationEntity>(new List<NotificationEntity>());
            _repositoryMock.Setup(r => r.Notifications).Returns(mockNotifications.AsQueryable());
            _notificationRegistrar.RegisterNotification<RegistrationEmailNotification>();

            //Act
            var result = await _notificationSearchService.GetNotificationAsync(type);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(type, result.Type);
        }

        [Fact]
        public async Task SearchNotificationsAsync_GetItems()
        {
            //Arrange
            var searchCriteria = AbstractTypeFactory<NotificationSearchCriteria>.TryCreateInstance();
            searchCriteria.Take = 20;
            _notificationRegistrar.RegisterNotification<RegistrationEmailNotification>();
            _notificationRegistrar.RegisterNotification<InvoiceEmailNotification>();
            _notificationRegistrar.RegisterNotification<OrderSentEmailNotification>();

            var notifications = new List<NotificationEntity>
            {
                new EmailNotificationEntity
                {
                    Type = nameof(RegistrationEmailNotification), Kind = nameof(EmailNotification),
                    Id = Guid.NewGuid().ToString(), IsActive = true
                }
            };

            var mockNotifications = new Common.TestAsyncEnumerable<NotificationEntity>(notifications);
            _repositoryMock.Setup(r => r.Notifications).Returns(mockNotifications.AsQueryable());
            var ids = notifications.Select(n => n.Id).ToArray();
            _notificationServiceMock.Setup(ns => ns.GetByIdsAsync(ids, null))
                .ReturnsAsync(notifications.Select(n => n.ToModel(AbstractTypeFactory<Notification>.TryCreateInstance(n.Type))).ToArray());

            //Act
            var result = await _notificationSearchService.SearchNotificationsAsync(searchCriteria);

            //Assert
            Assert.NotEmpty(result.Results);
            Assert.Equal(1, result.Results.Count(r => r.IsActive));
            //Assert.Equal(2, result.Results.Count(r => !r.IsActive));
        }

        [Fact]
        public async Task SearchNotificationsAsync_GetTwoItems()
        {
            //Arrange
            _notificationRegistrar.RegisterNotification<RegistrationEmailNotification>();
            _notificationRegistrar.RegisterNotification<InvoiceEmailNotification>();
            _notificationRegistrar.RegisterNotification<OrderSentEmailNotification>();
            var searchCriteria = AbstractTypeFactory<NotificationSearchCriteria>.TryCreateInstance();
            searchCriteria.Take = 2;
            searchCriteria.Skip = 0;
            var mockNotifications = new Common.TestAsyncEnumerable<NotificationEntity>(new List<NotificationEntity>());
            _repositoryMock.Setup(r => r.Notifications).Returns(mockNotifications.AsQueryable());

            //Act
            var result = await _notificationSearchService.SearchNotificationsAsync(searchCriteria);

            //Assert
            Assert.Equal(2, result.Results.Count);
        }

        [Fact]
        public async Task SearchNotificationsAsync_AllActiveNotifications()
        {
            //Arrange
            _notificationRegistrar.RegisterNotification<RegistrationEmailNotification>();
            _notificationRegistrar.RegisterNotification<InvoiceEmailNotification>();
            _notificationRegistrar.RegisterNotification<OrderSentEmailNotification>();

            var responseGroup = NotificationResponseGroup.Default.ToString();
            var searchCriteria = AbstractTypeFactory<NotificationSearchCriteria>.TryCreateInstance();
            searchCriteria.Take = 20;
            searchCriteria.ResponseGroup = responseGroup;
            var notificationEntities = new List<NotificationEntity> {
                new EmailNotificationEntity { Type  = nameof(InvoiceEmailNotification), Kind = nameof(EmailNotification), Id = Guid.NewGuid().ToString(), IsActive = true },
                new EmailNotificationEntity { Type  = nameof(OrderSentEmailNotification), Kind = nameof(EmailNotification), Id = Guid.NewGuid().ToString(), IsActive = true },
                new EmailNotificationEntity { Type  = nameof(RegistrationEmailNotification), Kind = nameof(EmailNotification), Id = Guid.NewGuid().ToString(), IsActive = true }
            };
            var mockNotifications = new Common.TestAsyncEnumerable<NotificationEntity>(notificationEntities);
            _repositoryMock.Setup(r => r.Notifications).Returns(mockNotifications.AsQueryable());
            var notifications = notificationEntities.Select(n => n.ToModel(AbstractTypeFactory<Notification>.TryCreateInstance(n.Type))).ToArray();
            var ids = notificationEntities.Select(n => n.Id).ToArray();
            _notificationServiceMock.Setup(ns => ns.GetByIdsAsync(ids, responseGroup))
                .ReturnsAsync(notifications);

            //Act
            var result = await _notificationSearchService.SearchNotificationsAsync(searchCriteria);

            //Assert
            Assert.True(result.Results.Where(n => ids.Contains(n.Id)).All(r => r.IsActive));
        }
    }
}
