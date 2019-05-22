using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.NotificationsModule.Data.Senders;
using VirtoCommerce.NotificationsModule.LiquidRenderer;
using VirtoCommerce.NotificationsModule.Smtp;
using VirtoCommerce.NotificationsModule.Tests.Common;
using VirtoCommerce.NotificationsModule.Tests.Model;
using VirtoCommerce.NotificationsModule.Tests.NotificationTypes;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Notifications;
using Xunit;

namespace VirtoCommerce.NotificationsModule.Tests.UnitTests
{
    public class NotificationSenderUnitTests
    {

        private readonly NotificationSender _sender;
        private readonly Mock<INotificationService> _serviceMock;
        private readonly INotificationTemplateRenderer _templateRender;
        private readonly Mock<INotificationMessageService> _messageServiceMock;
        private readonly Mock<INotificationMessageSender> _messageSenderMock;
        private readonly Mock<ILogger<NotificationSender>> _logNotificationSenderMock;
        private readonly Mock<SmtpEmailNotificationMessageSender> _smtpEmailNotificationMessageSenderMock;
        private readonly INotificationMessageSenderProviderFactory _senderFactory;
        private readonly Mock<INotificationMessageSenderProviderFactory> _senderFactoryMock;
        private readonly Mock<IOptions<EmailSendingOptions>> _emailSendingOptionsMock;

        public NotificationSenderUnitTests()
        {
            _templateRender = new LiquidTemplateRenderer();
            _messageServiceMock = new Mock<INotificationMessageService>();
            _messageSenderMock = new Mock<INotificationMessageSender>();
            _serviceMock = new Mock<INotificationService>();
            _logNotificationSenderMock = new Mock<ILogger<NotificationSender>>();
            _emailSendingOptionsMock = new Mock<IOptions<EmailSendingOptions>>();

            _senderFactoryMock = new Mock<INotificationMessageSenderProviderFactory>();
            _smtpEmailNotificationMessageSenderMock = new Mock<SmtpEmailNotificationMessageSender>(_emailSendingOptionsMock.Object);
            _senderFactoryMock.Setup(s => s.GetSenderForNotificationType(nameof(EmailNotification))).Returns(_messageSenderMock.Object);

            _sender = new NotificationSender(_templateRender, _messageServiceMock.Object, _logNotificationSenderMock.Object, _senderFactoryMock.Object);

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
            string language = "en-US";
            string subject = "Your order was sent";
            string body = "Your order <strong>{{ customer_order.number}}</strong> was sent.<br> Number of sent parcels - " +
                          "<strong>{{ customer_order.shipments | size}}</strong>.<br> Parcels tracking numbers:<br> {% for shipment in customer_order.shipments %} " +
                          "<br><strong>{{ shipment.number}}</strong> {% endfor %}<br><br>Sent date - <strong>{{ customer_order.modified_date }}</strong>.";
            var notification = new OrderSentEmailNotification()
            {
                CustomerOrder = new CustomerOrder()
                {
                    Number = "123"
                    ,
                    Shipments = new[] { new Shipment() { Number = "some_number" } }
                    ,
                    ModifiedDate = DateTime.Now
                },
                Templates = new List<NotificationTemplate>()
                {
                    new EmailNotificationTemplate()
                    {
                        Subject = subject,
                        Body = body,
                        LanguageCode = "en-US"
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
            //_serviceMock.Setup(serv => serv.GetByTypeAsync(nameof(OrderSentEmailNotification), null, null, null)).ReturnsAsync(notification);
            _messageServiceMock.Setup(ms => ms.SaveNotificationMessagesAsync(new NotificationMessage[] { message }));

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
            string subject = "Invoice for order - <strong>{{ customer_order.number }}</strong>";
            string body = TestUtility.GetStringByPath($"Content\\{nameof(InvoiceEmailNotification)}.html");
            var notification = new InvoiceEmailNotification()
            {
                CustomerOrder = new CustomerOrder()
                {
                    Id = "adsffads",
                    Number = "123",
                    ShippingTotal = 123456.789m,
                    CreatedDate = DateTime.Now,
                    Status = "Paid",
                    Total = 123456.789m,
                    FeeTotal = 123456.789m,
                    SubTotal = 123456.789m,
                    TaxTotal = 123456.789m,
                    Currency = "USD",
                    Items = new[]{ new LineItem
                    {
                        Name = "some",
                        Sku = "sku",
                        PlacedPrice = 12345.6789m,
                        Quantity = 1,
                        ExtendedPrice = 12345.6789m,
                    } }
                },
                Templates = new List<NotificationTemplate>()
                {
                    new EmailNotificationTemplate()
                    {
                        Subject = subject,
                        Body = body,
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
            //_serviceMock.Setup(serv => serv.GetByTypeAsync(nameof(InvoiceEmailNotification), null, null, null)).ReturnsAsync(notification);
            _messageServiceMock.Setup(ms => ms.SaveNotificationMessagesAsync(new NotificationMessage[] { message }));


            //Act
            var result = await _sender.SendNotificationAsync(notification, language);

            //Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task RegistrationEmailNotification_SentNotification()
        {
            //Arrange
            string language = "default";
            string subject = "Your login - {{ login }}.";
            string body = "Thank you for registration {{ firstname }} {{ lastname }}!!!";
            var notification = new RegistrationEmailNotification()
            {
                FirstName = "First Name",
                LastName = "Last Name",
                Login = "Test login",
                Templates = new List<NotificationTemplate>()
                {
                    new EmailNotificationTemplate()
                    {
                        Subject = subject,
                        Body = body,
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
            //_serviceMock.Setup(serv => serv.GetByTypeAsync(nameof(RegistrationEmailNotification), null, null, null)).ReturnsAsync(notification);
            _messageServiceMock.Setup(ms => ms.SaveNotificationMessagesAsync(new NotificationMessage[] { message }));

            //Act
            var result = await _sender.SendNotificationAsync(notification, language);

            //Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task OrderPaidEmailNotification_SentNotification()
        {
            //Arrange
            string language = "default";
            string subject = "Your order was fully paid";
            string body = "Thank you for paying <strong>{{ customer_order.number }}</strong> order.<br> " +
                          "You had paid <strong>{{ customer_order.total | math.format 'N'}} {{ customer_order.currency }}</strong>.<br>" +
                          " Paid date - <strong>{{ customer_order.modified_date }}</strong>.";
            var notification = new OrderPaidEmailNotification()
            {
                CustomerOrder = new CustomerOrder()
                {
                    Number = "123",
                    Total = 1234.56m,
                    Currency = "USD",
                    ModifiedDate = DateTime.Now
                },
                Templates = new List<NotificationTemplate>()
                {
                    new EmailNotificationTemplate()
                    {
                        Subject = subject,
                        Body = body,
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
            //_serviceMock.Setup(serv => serv.GetByTypeAsync(nameof(OrderPaidEmailNotification), null, null, null)).ReturnsAsync(notification);
            _messageServiceMock.Setup(ms => ms.SaveNotificationMessagesAsync(new NotificationMessage[] { message }));

            //Act
            var result = await _sender.SendNotificationAsync(notification, language);

            //Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task EmailNotification_SuccessSend()
        {
            //Arrange
            string language = null;
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
                    }
                },
                TenantIdentity = new TenantIdentity(null, null)
            };

            var message = new EmailNotificationMessage()
            {
                Id = "1",
                From = "from@from.com",
                To = "to@to.com",
                Subject = subject,
                Body = body,
                SendDate = DateTime.Now,
                TenantIdentity = new TenantIdentity(null, null)
            };
            //_serviceMock.Setup(serv => serv.GetByTypeAsync(nameof(EmailNotification), null, null, null)).ReturnsAsync(notification);

            _messageServiceMock.Setup(ms => ms.SaveNotificationMessagesAsync(new NotificationMessage[] { message }));

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
            //_serviceMock.Setup(serv => serv.GetByTypeAsync(nameof(EmailNotification), null, null, null)).ReturnsAsync(notification);
            _messageServiceMock.Setup(ms => ms.SaveNotificationMessagesAsync(new NotificationMessage[] { message }));
            _messageSenderMock.Setup(ms => ms.SendNotificationAsync(It.IsAny<NotificationMessage>())).Throws(new SmtpException());

            //Act
            var result = await _sender.SendNotificationAsync(notification, language);

            //Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task EmailNotification_ArgumentNullException()
        {
            //Arrange
            string language = "default";
            NotificationMessage message = null;
            _messageServiceMock.Setup(ms => ms.SaveNotificationMessagesAsync(new[] { message }));

            //Act
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sender.SendNotificationAsync(null, language));
        }

        [Fact]
        public async Task SendNotificationAsync_SentEmailNotification()
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
            //_serviceMock.Setup(serv => serv.GetByTypeAsync(nameof(EmailNotification), null, null, null)).ReturnsAsync(notification);
            _messageServiceMock.Setup(ms => ms.SaveNotificationMessagesAsync(new NotificationMessage[] { message }));
            _messageSenderMock.Setup(ms => ms.SendNotificationAsync(It.IsAny<NotificationMessage>())).Throws(new SmtpException());

            var sender = new NotificationSender(_templateRender, _messageServiceMock.Object, _logNotificationSenderMock.Object, _senderFactory);

            //Act
            var result = await sender.SendNotificationAsync(notification, language);

            //Assert
            Assert.False(result.IsSuccess);
        }
    }
}
