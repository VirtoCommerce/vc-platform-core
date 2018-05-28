using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.NotificationsModule.Data.Senders;
using VirtoCommerce.NotificationsModule.Data.Services;
using VirtoCommerce.NotificationsModule.LiquidRenderer;
using VirtoCommerce.NotificationsModule.Smtp;
using VirtoCommerce.NotificationsModule.Tests.Model;
using VirtoCommerce.NotificationsModule.Tests.NotificationTypes;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Notifications;
using Xunit;

namespace VirtoCommerce.NotificationsModule.Tests.IntegrationTests
{
    public class NotificationSenderIntegrationTests
    {
        INotificationMessageSender _messageSender;
        NotificationSender _notificationSender;
        private readonly Mock<INotificationService> _serviceMock;
        private readonly INotificationTemplateRender _templateRender;
        private readonly Mock<INotificationMessageService> _messageServiceMock;
        private readonly Mock<IOptions<EmailSendingOptions>> _emailSendingOptionsMock;
        private readonly INotificationRegistrar _notificationRegistrar;
        private readonly Mock<INotificationRepository> _repositoryMock;
        private readonly Mock<ILogger<NotificationSender>> _logNotificationSenderMock;
        private readonly Mock<IEventPublisher> _eventPulisherMock;
        private INotificationMessageSenderProviderFactory _notificationMessageSenderProviderFactory;
        private readonly EmailSendingOptions _emailSendingOptions;

        public NotificationSenderIntegrationTests()
        {
            _emailSendingOptions = new EmailSendingOptions()
            {
                    SmtpServer = "smtp.gmail.com",
                    Port = 587,
                    Login = "tasker.for.test@gmail.com",
                    Password = "FLGxiJQc"
            };
            _templateRender = new LiquidTemplateRenderer();
            _messageServiceMock = new Mock<INotificationMessageService>();
            _emailSendingOptionsMock = new Mock<IOptions<EmailSendingOptions>>();
            _serviceMock = new Mock<INotificationService>();
            _repositoryMock = new Mock<INotificationRepository>();
            _logNotificationSenderMock = new Mock<ILogger<NotificationSender>>();
            _eventPulisherMock = new Mock<IEventPublisher>();
            INotificationRepository RepositoryFactory() => _repositoryMock.Object;
            _notificationRegistrar = new NotificationService(RepositoryFactory, _eventPulisherMock.Object);
            
            

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

            _notificationRegistrar.RegisterNotification<RegistrationEmailNotification>();
        }

        [Fact]
        public async Task SmtpEmailNotificationMessageSender_SuccessSentMessage()
        {
            //Arrange
            string number = Guid.NewGuid().ToString();
            string subject = "Order #{{customer_order.number}}";
            string body = "You have order #{{customer_order.number}}";
            var notification = new OrderSentEmailNotification()
            {
                CustomerOrder = new CustomerOrder() { Number = number },
                From = "tasker.for.test@gmail.com",
                To = "tasker.for.test@gmail.com",
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

            _serviceMock.Setup(serv => serv.GetByTypeAsync(nameof(OrderSentEmailNotification), null, null, null)).ReturnsAsync(notification);

            
            _emailSendingOptionsMock.Setup(opt => opt.Value).Returns(_emailSendingOptions);
            _messageSender = new SmtpEmailNotificationMessageSender(_emailSendingOptionsMock.Object);
            _notificationMessageSenderProviderFactory = new NotificationMessageSenderProviderFactory(new List<INotificationMessageSender>() { _messageSender });
            _notificationMessageSenderProviderFactory.RegisterSenderForType<EmailNotification, SmtpEmailNotificationMessageSender>();
            _notificationSender = new NotificationSender(_templateRender, _messageServiceMock.Object, _logNotificationSenderMock.Object, _notificationMessageSenderProviderFactory);

            //Act
            var result = await _notificationSender.SendNotificationAsync(notification, null);

            //Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task SmtpEmailNotificationMessageSender_FailSendMessage()
        {
            //Arrange
            string language = null;
            string number = Guid.NewGuid().ToString();
            string subject = "Order #{{customer_order.number}}";
            string body = "You have order #{{customer_order.number}}";
            var notification = new OrderSentEmailNotification()
            {
                CustomerOrder = new CustomerOrder() { Number = number },
                From = "tasker.for.test@gmail.com",
                To = "tasker.for.test@gmail.com",
                Templates = new List<NotificationTemplate>()
                {
                    new EmailNotificationTemplate()
                    {
                        Subject = subject,
                        Body = body,
                    }
                }
            };

            _serviceMock.Setup(serv => serv.GetByTypeAsync(nameof(OrderSentEmailNotification), null, null, null)).ReturnsAsync(notification);

            _emailSendingOptions.Password = "wrong_password";
            _emailSendingOptionsMock.Setup(opt => opt.Value).Returns(_emailSendingOptions);
            _messageSender = new SmtpEmailNotificationMessageSender(_emailSendingOptionsMock.Object);
            _notificationSender = new NotificationSender(_templateRender, _messageServiceMock.Object, _logNotificationSenderMock.Object,
                _notificationMessageSenderProviderFactory);

            //Act
            var result = await _notificationSender.SendNotificationAsync(notification, language);

            //Assert
            Assert.False(result.IsSuccess);
        }
    }
}
