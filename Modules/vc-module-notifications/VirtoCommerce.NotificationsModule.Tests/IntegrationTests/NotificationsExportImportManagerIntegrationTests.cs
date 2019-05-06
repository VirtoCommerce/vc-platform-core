using System;
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
        private readonly NotificationSearchService _notificationSearchService;
        private readonly NotificationService _notificationService;
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
            _notificationSearchService = new NotificationSearchService(RepositoryFactory);
            _notificationService = new NotificationService(RepositoryFactory, _eventPulisherMock.Object);

            var mvcJsonOptions = new MvcJsonOptions()
            {
                SerializerSettings =
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.Indented
                }
            };

            _notificationsExportImportManager = new NotificationsExportImport(_notificationSearchService, _notificationService, JsonSerializer.Create(mvcJsonOptions.SerializerSettings));

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

        [Fact]
        public async Task DoExport_SuccessExport()
        {
            //Arrange
            var manifest = new PlatformExportManifest();
            var fileStream = new FileStream(Path.GetFullPath("export_test.json"), FileMode.Create);
            var entity = AbstractTypeFactory<NotificationEntity>.TryCreateInstance(nameof(EmailNotificationEntity));
            entity.Id = Guid.NewGuid().ToString();
            entity.Type = nameof(RegistrationEmailNotification);
            entity.Kind = nameof(EmailNotification);
            entity.TenantId = Guid.NewGuid().ToString();
            entity.TenantType = nameof(Notification);
            entity.Templates = new ObservableCollection<NotificationTemplateEntity>()
            {
                new NotificationTemplateEntity() { Body = "test", LanguageCode = "en-US" },
            };

            _repositoryMock.Setup(r => r.GetByTypesAsync(new[] { nameof(RegistrationEmailNotification) }, null, null, NotificationResponseGroup.Default.ToString(), true))
                           .ReturnsAsync(new[] { entity });

            _repositoryMock.Setup(r => r.GetByTypesAsync(new[] { nameof(RegistrationEmailNotification) }, null, null, NotificationResponseGroup.Full.ToString(), true))
                           .ReturnsAsync(new[] { entity });

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
