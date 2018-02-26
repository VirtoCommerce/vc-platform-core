using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.NotificationsModule.Notifications.NotificationTypes;
using VirtoCommerce.NotificationsModule.Notifications.Rendering;
using VirtoCommerce.NotificationsModule.Notifications.Senders;
using Xunit;

namespace VirtoCommerce.NotificationsModule.Tests
{
    public class NotificationSenderTests
    {
        
        private readonly NotificationSender _sender;
        private readonly Mock<INotificationService> _serviceMock;
        private readonly Mock<INotificationTemplateRender> _templateRenderMock;
        private readonly INotificationTemplateRender _templateRender;
        private readonly Mock<INotificationMessageService> _messageServiceMock;
        private readonly Mock<INotificationMessageSender> _messageSenderMock;
        private readonly Mock<INotificationRepository> _repositoryMock;
        private readonly Mock<INotificationSender> _senderMock;
        private readonly Mock<INotificationRegistrar> _registrarMock;


        public NotificationSenderTests()
        {
            _repositoryMock = new Mock<INotificationRepository>();
            Func<INotificationRepository> repositoryFactory = () => _repositoryMock.Object;
            _templateRenderMock = new Mock<INotificationTemplateRender>();
            _templateRender = new LiquidTemplateRenderer();
            _messageServiceMock = new Mock<INotificationMessageService>();
            _messageSenderMock = new Mock<INotificationMessageSender>();
            _senderMock = new Mock<INotificationSender>();
            _registrarMock = new Mock<INotificationRegistrar>();
            _serviceMock = new Mock<INotificationService>();
            _sender = new NotificationSender(_serviceMock.Object, _templateRender, _messageServiceMock.Object, _messageSenderMock.Object);
        }

        [Fact]
        public async Task OrderSendEmailNotification_SentNotification()
        {
            //Arrange
            string language = "default";
            string subject = "Order #{{order.id}}";
            string body = "You have order #{{order.id}}";
            var notification = new OrderSentEmailNotification()
            {
                Order = new CustomerOrder() { Id = "123"},
                Templates = new List<NotificationTemplate>()
                {
                    new EmailNotificationTemplate()
                    {
                        Subject = subject,
                        Body = body,
                        LanguageCode = language
                    }
                }
            };
            var date = new DateTime(2018, 02, 20, 10, 00, 00);
            var message = new EmailNotificationMessage()
            {
                Id = "1",
                From = "from@from.com",
                To = "to@to.com",
                Subject = subject,
                Body = body,
                SendDate = date
            };
            _serviceMock.Setup(serv => serv.GetNotificationByTypeAsync(nameof(OrderSentEmailNotification), null)).ReturnsAsync(notification);
            //_templateRenderMock.Setup(tr => tr.Render(message.Subject, notification.Order)).Returns("Order #123");
            //_templateRenderMock.Setup(tr => tr.Render(message.Body, notification.Order)).Returns("You have order #123");
            _messageServiceMock.Setup(ms => ms.SaveNotificationMessages(new NotificationMessage[] {message}));

            //Act
            await _sender.SendNotificationAsync(notification, language);

            //Assert
        }

        [Fact]
        public async Task InvoiceEmailNotification_SentNotification()
        {
            //Arrange
            string language = "default";
            string subject = "Invoice for order - {{ order.number }} {{ order.created_date }}";
            string body = "total: {{ order.shipping_total | math.format 'C' }}";
            var notification = new InvoiceEmailNotification()
            {
                Order = new CustomerOrder() { Id = "adsffads", Number = "123", ShippingTotal = 123456.789m, CreatedDate = DateTime.Now },
                Templates = new List<NotificationTemplate>()
                {
                    new EmailNotificationTemplate()
                    {
                        Subject = subject,
                        Body = body,
                        LanguageCode = language
                    }
                }
            };
            var date = new DateTime(2018, 02, 20, 10, 00, 00);
            var message = new EmailNotificationMessage()
            {
                Id = "1",
                From = "from@from.com",
                To = "to@to.com",
                Subject = subject,
                Body = body,
                SendDate = date
            };
            _serviceMock.Setup(serv => serv.GetNotificationByTypeAsync(nameof(InvoiceEmailNotification), null)).ReturnsAsync(notification);
            _messageServiceMock.Setup(ms => ms.SaveNotificationMessages(new NotificationMessage[] { message }));

            //Act
            await _sender.SendNotificationAsync(notification, language);

            //Assert
        }
    }
}
