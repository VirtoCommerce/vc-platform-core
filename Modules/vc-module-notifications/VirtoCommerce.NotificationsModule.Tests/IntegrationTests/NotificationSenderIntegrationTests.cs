using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Core.Types;
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
using Xunit;

namespace VirtoCommerce.NotificationsModule.Tests.IntegrationTests
{
    public class NotificationSenderIntegrationTests
    {
        private INotificationMessageSender _messageSender;
        private NotificationSender _notificationSender;
        private readonly Mock<INotificationService> _serviceMock;
        private readonly INotificationTemplateRenderer _templateRender;
        private readonly Mock<INotificationMessageService> _messageServiceMock;
        private readonly Mock<IOptions<SmtpSenderOptions>> _emailSendingOptionsMock;
        private readonly INotificationRegistrar _notificationRegistrar;
        private readonly Mock<INotificationRepository> _repositoryMock;
        private readonly Mock<ILogger<NotificationSender>> _logNotificationSenderMock;
        private readonly Mock<IEventPublisher> _eventPulisherMock;
        private INotificationMessageSenderProviderFactory _notificationMessageSenderProviderFactory;
        private readonly SmtpSenderOptions _emailSendingOptions;

        public NotificationSenderIntegrationTests()
        {
            _emailSendingOptions = new SmtpSenderOptions()
            {
                SmtpServer = "http://smtp.gmail.com/",
                Port = 587,
                Login = "tasker.for.test@gmail.com",
                Password = ""
            };
            _templateRender = new LiquidTemplateRenderer();
            _messageServiceMock = new Mock<INotificationMessageService>();
            _emailSendingOptionsMock = new Mock<IOptions<SmtpSenderOptions>>();
            _serviceMock = new Mock<INotificationService>();
            _repositoryMock = new Mock<INotificationRepository>();
            _logNotificationSenderMock = new Mock<ILogger<NotificationSender>>();
            _eventPulisherMock = new Mock<IEventPublisher>();
            INotificationRepository RepositoryFactory() => _repositoryMock.Object;
            _notificationRegistrar = new NotificationService(RepositoryFactory, _eventPulisherMock.Object);

            if (!AbstractTypeFactory<NotificationTemplate>.AllTypeInfos.SelectMany(x => x.AllSubclasses).Contains(typeof(EmailNotificationTemplate)))
                AbstractTypeFactory<NotificationTemplate>.RegisterType<EmailNotificationTemplate>().MapToType<NotificationTemplateEntity>();

            if (!AbstractTypeFactory<NotificationMessage>.AllTypeInfos.SelectMany(x => x.AllSubclasses).Contains(typeof(EmailNotificationMessage)))
                AbstractTypeFactory<NotificationMessage>.RegisterType<EmailNotificationMessage>().MapToType<NotificationMessageEntity>();

            _notificationRegistrar.RegisterNotification<RegistrationEmailNotification>();
            _notificationRegistrar.RegisterNotification<ResetPasswordEmailNotification>();
        }

        [Fact]
        public async Task SmtpEmailNotificationMessageSender_SuccessSentMessage()
        {
            //Arrange
            var number = Guid.NewGuid().ToString();
            var subject = "Order #{{customer_order.number}}";
            var body = "You have order #{{customer_order.number}}";
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
            var number = Guid.NewGuid().ToString();
            var subject = "Order #{{customer_order.number}}";
            var body = "You have order #{{customer_order.number}}";
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
