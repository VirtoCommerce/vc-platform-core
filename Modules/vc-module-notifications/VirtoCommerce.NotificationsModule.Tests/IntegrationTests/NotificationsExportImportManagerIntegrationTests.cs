using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Data.ExportImport;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.NotificationsModule.Data.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.ExportImport;
using Xunit;
using RegistrationEmailNotification = VirtoCommerce.NotificationsModule.Tests.NotificationTypes.RegistrationEmailNotification;

namespace VirtoCommerce.NotificationsModule.Tests.IntegrationTests
{
    public class NotificationsExportImportManagerIntegrationTests
    {
        private NotificationsExportImport _notificationsExportImportManager;
        private readonly INotificationRegistrar _notificationRegistrar;
        private readonly Mock<INotificationSearchService> _notificationSearchServiceMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<INotificationRepository> _repositoryMock;
        private readonly Mock<IEventPublisher> _eventPulisherMock;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;

        public NotificationsExportImportManagerIntegrationTests()
        {
            _repositoryMock = new Mock<INotificationRepository>();
            _eventPulisherMock = new Mock<IEventPublisher>();
            INotificationRepository RepositoryFactory() => _repositoryMock.Object;
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _repositoryMock.Setup(ss => ss.UnitOfWork).Returns(_mockUnitOfWork.Object);
            _notificationSearchServiceMock = new Mock<INotificationSearchService>();
            _notificationServiceMock = new Mock<INotificationService>();

            _notificationsExportImportManager = new NotificationsExportImport(_notificationSearchServiceMock.Object, _notificationServiceMock.Object, GetJsonSerializer());

            _notificationRegistrar = new NotificationService(RepositoryFactory, _eventPulisherMock.Object);


            if (!AbstractTypeFactory<Notification>.AllTypeInfos.Any(t => t.IsAssignableTo(nameof(EmailNotification))))
            {
                AbstractTypeFactory<Notification>.RegisterType<EmailNotification>().MapToType<NotificationEntity>();
                AbstractTypeFactory<NotificationEntity>.RegisterType<EmailNotificationEntity>();
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

        private JsonSerializer GetJsonSerializer()
        {
            return JsonSerializer.Create(new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });
        }

        [Fact]
        public async Task DoExport_SuccessExport()
        {
            //Arrange
            var manifest = new PlatformExportManifest();
            var fileStream = new FileStream(Path.GetFullPath("export_test.json"), FileMode.Create);
            var entity = AbstractTypeFactory<Notification>.TryCreateInstance(nameof(EmailNotification));
            entity.Id = Guid.NewGuid().ToString();
            entity.Type = nameof(RegistrationEmailNotification);
            entity.Kind = nameof(EmailNotification);
            entity.TenantIdentity = new TenantIdentity(Guid.NewGuid().ToString(), nameof(Notification));
            entity.Templates = new ObservableCollection<NotificationTemplate>()
            {
                new EmailNotificationTemplate() { Body = "test", LanguageCode = "en-US" },
            };

            _notificationSearchServiceMock
                .Setup(nss => nss.SearchNotificationsAsync(It.IsAny<NotificationSearchCriteria>()))
                .ReturnsAsync(new NotificationSearchResult {Results = new List<Notification> { entity } , TotalCount = 1});

            //Act
            await _notificationsExportImportManager.DoExportAsync(fileStream, exportImportProgressInfo => { }, new CancellationTokenWrapper(CancellationToken.None));

            //Assert
            fileStream.Close();
        }

        [Fact]
        public async Task DoImport_SuccessImport()
        {
            //Arrange
            var manifest = new PlatformExportManifest();
            var fileStream = new FileStream(Path.GetFullPath("export_test.json"), FileMode.Open);

            //Act
            await _notificationsExportImportManager.DoImportAsync(fileStream, exportImportProgressInfo => { }, new CancellationTokenWrapper(CancellationToken.None));

            //Assert
            fileStream.Close();
        }
    }
}
