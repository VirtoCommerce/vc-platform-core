using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.NotificationsModule.Data.Senders;
using VirtoCommerce.NotificationsModule.LiguidRenderer;
using VirtoCommerce.NotificationsModule.Tests.NotificationTypes;
using VirtoCommerce.Platform.Core.Common;
using Xunit;

namespace VirtoCommerce.NotificationsModule.Tests
{
    public class NotificationSenderUnitTests
    {
        
        private readonly NotificationSender _sender;
        private readonly Mock<INotificationService> _serviceMock;
        private readonly INotificationTemplateRender _templateRender;
        private readonly Mock<INotificationMessageService> _messageServiceMock;
        private readonly Mock<INotificationMessageSender> _messageSenderMock;
        private readonly Mock<ILogger<NotificationSender>> _logNotificationSenderMock;

        public NotificationSenderUnitTests()
        {
            _templateRender = new LiquidTemplateRenderer();
            _messageServiceMock = new Mock<INotificationMessageService>();
            _messageSenderMock = new Mock<INotificationMessageSender>();
            _serviceMock = new Mock<INotificationService>();
            _logNotificationSenderMock = new Mock<ILogger<NotificationSender>>();
            _sender = new NotificationSender(_serviceMock.Object, _templateRender, _messageServiceMock.Object, _messageSenderMock.Object, _logNotificationSenderMock.Object);

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
            _serviceMock.Setup(serv => serv.GetNotificationByTypeAsync(nameof(OrderSentEmailNotification), null, null)).ReturnsAsync(notification);
            _messageServiceMock.Setup(ms => ms.SaveNotificationMessages(new NotificationMessage[] {message}));

            //Act
            var result = await _sender.SendNotificationAsync(notification, language);

            //Assert
            Assert.True(result.IsSuccess);
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
            _serviceMock.Setup(serv => serv.GetNotificationByTypeAsync(nameof(InvoiceEmailNotification), null, null)).ReturnsAsync(notification);
            _messageServiceMock.Setup(ms => ms.SaveNotificationMessages(new NotificationMessage[] { message }));

            //Act
            var result = await _sender.SendNotificationAsync(notification, language);

            //Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task EmailNotification_SuccessSend()
        {
            //Arrange
            string language = "default";
            string subject = "some subject";
            string body = "some body";
            var notification = new EmailNotification()
            {
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
            
            var message = new EmailNotificationMessage()
            {
                Id = "1",
                From = "from@from.com",
                To = "to@to.com",
                Subject = subject,
                Body = body,
                SendDate = DateTime.Now
            };
            _serviceMock.Setup(serv => serv.GetNotificationByTypeAsync(nameof(EmailNotification), null, null)).ReturnsAsync(notification);

            _messageServiceMock.Setup(ms => ms.SaveNotificationMessages(new NotificationMessage[] { message }));

            //Act
            var result = await _sender.SendNotificationAsync(notification, language);

            //Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task EmailNotification_FailSend()
        {
            //Arrange
            string language = "default";
            string subject = "some subject";
            string body = "some body";
            var notification = new EmailNotification()
            {
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

            var message = new EmailNotificationMessage()
            {
                Id = "1",
                From = "from@from.com",
                To = "to@to.com",
                Subject = subject,
                Body = body,
                SendDate = DateTime.Now
            };
            _serviceMock.Setup(serv => serv.GetNotificationByTypeAsync(nameof(EmailNotification), null, null)).ReturnsAsync(notification);
            _messageServiceMock.Setup(ms => ms.SaveNotificationMessages(new NotificationMessage[] { message }));
            _messageSenderMock.Setup(ms => ms.SendNotificationAsync(It.IsAny<NotificationMessage>())).Throws(new SmtpException());

            //Act
            var result = await _sender.SendNotificationAsync(notification, language);

            //Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EmailNotification_NullReferenceException()
        {
            //Arrange
            string language = "default";
            string subject = "some subject";
            string body = "some body";
            var notification = new EmailNotification()
            {
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

            NotificationMessage message = null;
            _messageServiceMock.Setup(ms => ms.SaveNotificationMessages(new [] { message }));

            //Act
            await Assert.ThrowsAsync<NullReferenceException>(() => _sender.SendNotificationAsync(notification, language));
        }

        [Fact]
        public async Task EmailNotification_ArgumentNullException()
        {
            //Arrange
            string language = "default";
            NotificationMessage message = null;
            _messageServiceMock.Setup(ms => ms.SaveNotificationMessages(new[] { message }));

            //Act
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sender.SendNotificationAsync(null, language));
        }
    }
}
