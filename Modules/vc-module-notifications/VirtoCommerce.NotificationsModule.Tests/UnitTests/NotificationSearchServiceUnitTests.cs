using System;
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
        private readonly NotificationSearchService _notificationSearchService;

        public NotificationSearchServiceUnitTests()
        {
            _repositoryMock = new Mock<INotificationRepository>();
            _repositoryFactoryMock = new Mock<Func<INotificationRepository>>();
            _eventPublisherMock = new Mock<IEventPublisher>();
            _notificationRegistrar = new NotificationService(_repositoryFactoryMock.Object, _eventPublisherMock.Object);
            _repositoryFactory = () => _repositoryMock.Object;
            _notificationSearchService = new NotificationSearchService(_repositoryFactory);
        }

        [Fact]
        public void SearchNotificationsAsync_GetNotifications()
        {
            //Arrange
            var searchCriteria = new NotificationSearchCriteria();
            _notificationRegistrar.RegisterNotification<RegistrationEmailNotification>();
            _notificationRegistrar.RegisterNotification<InvoiceEmailNotification>();
            _notificationRegistrar.RegisterNotification<OrderSentEmailNotification>();

            //Act
            var result = _notificationSearchService.SearchNotifications(searchCriteria);

            //Assert
            Assert.NotEmpty(result.Results);
        }

        [Fact]
        public void SearchNotificationsAsync_Get2Items()
        {
            //Arrange
            _notificationRegistrar.RegisterNotification<RegistrationEmailNotification>();
            _notificationRegistrar.RegisterNotification<InvoiceEmailNotification>();
            _notificationRegistrar.RegisterNotification<OrderSentEmailNotification>();
            var searchCriteria = new NotificationSearchCriteria
            {
                Take = 2,
                Skip = 0
            };

            //Act
            var result = _notificationSearchService.SearchNotifications(searchCriteria);

            //Assert
            Assert.Equal(2, result.Results.Count);
        }

        [Fact]
        public void SearchNotificationsAsync_ContainsActiveNotications()
        {
            //Arrange
            _notificationRegistrar.RegisterNotification<RegistrationEmailNotification>();
            _notificationRegistrar.RegisterNotification<InvoiceEmailNotification>();
            _notificationRegistrar.RegisterNotification<OrderSentEmailNotification>();
            var searchCriteria = new NotificationSearchCriteria();
            _repositoryMock.Setup(n => n.GetEntityForListByType(nameof(OrderSentEmailNotification), null, null))
                .Returns(new EmailNotificationEntity { IsActive = true });

            //Act
            var result = _notificationSearchService.SearchNotifications(searchCriteria);

            //Assert
            Assert.Contains(result.Results, n => n.IsActive);
        }
    }
}
