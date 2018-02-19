using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.NotificationsModule.Core.NotificationTypes;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.NotificationsModule.Data.Sender;
using VirtoCommerce.NotificationsModule.Data.Services;
using Xunit;

namespace VirtoCommerce.NotificationsModule.Tests
{
    public class NotificationSenderTests
    {
        
        private readonly NotificationSender _sender;
        private readonly Mock<INotificationService> _serviceMock;
        private readonly Mock<INotificationRepository> _repositoryMock;
        private readonly Mock<INotificationSender> _senderMock;
        private readonly Mock<INotificationRegistrar> _registrarMock;


        public NotificationSenderTests()
        {
            _repositoryMock = new Mock<INotificationRepository>();
            Func<INotificationRepository> repositoryFactory = () => _repositoryMock.Object;
            _senderMock = new Mock<INotificationSender>();
            _registrarMock = new Mock<INotificationRegistrar>();
            _serviceMock = new Mock<INotificationService>();
            _sender = new NotificationSender(_serviceMock.Object);
        }

        [Fact]
        public async Task OrderSendEmailNotification_SentNotification()
        {
            //Arrange
            var notification = new OrderSendEmailNotification()
            {
                Order = new { orderId = "orderId" }
            };
            _registrarMock.Setup(rm => rm.RegisterNotification<OrderSendEmailNotification>());

            //Act
            
            await _sender.SendNotificationAsync(notification, "default");

            //Assert
        }
    }
}
