using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using Moq;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.NotificationsModule.Data.Services;
using VirtoCommerce.NotificationsModule.Tests.NotificationTypes;
using VirtoCommerce.NotificationsModule.Web.ExportImport;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.ExportImport;
using Xunit;

namespace VirtoCommerce.NotificationsModule.Tests.IntegrationTests
{
    public class NotificationsExportImportManagerIntegrationTests
    {
        private NotificationsExportImportManager _notificationsExportImportManager;
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
            _notificationsExportImportManager = new NotificationsExportImportManager(_notificationSearchService, _notificationService);
            
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
        public void DoExport_SuccessExport()
        {
            //Arrange
            var manifest = new PlatformExportManifest();
            var fileStream = new FileStream("c:\\vc\\export_test.json", FileMode.Create);
            var entityForCount = AbstractTypeFactory<NotificationEntity>.TryCreateInstance(nameof(EmailNotificationEntity));
            entityForCount.Id = Guid.NewGuid().ToString();
            entityForCount.Type = nameof(EmailNotification);
            entityForCount.Kind = nameof(EmailNotification);
            _repositoryMock.Setup(r => r.GetByTypeAsync(nameof(RegistrationEmailNotification), null, null, NotificationResponseGroup.Default)).ReturnsAsync(entityForCount);

            var entity = AbstractTypeFactory<NotificationEntity>.TryCreateInstance(nameof(EmailNotificationEntity));
            entity.Id = entityForCount.Id;
            entity.Type = nameof(EmailNotification);
            entity.Kind = nameof(EmailNotification);
            entity.Templates = new ObservableCollection<NotificationTemplateEntity>()
            {
                new NotificationTemplateEntity() { Body = "test", LanguageCode = "en-US" }
            };
            _repositoryMock.Setup(r => r.GetByTypeAsync(nameof(RegistrationEmailNotification), null, null, NotificationResponseGroup.Full)).ReturnsAsync(entity);

            //Act
            _notificationsExportImportManager.DoExport(fileStream, manifest, exportImportProgressInfo => {}, CancellationToken.None);

            //Assert
            fileStream.Close();
        }

        [Fact]
        public void DoImport_SuccessImport()
        {
            //Arrange
            var manifest = new PlatformExportManifest();
            var fileStream = new FileStream("c:\\vc\\export_test.json", FileMode.Open);
            
            //Act
            _notificationsExportImportManager.DoImport(fileStream, manifest, exportImportProgressInfo => { }, CancellationToken.None);

            //Assert
            fileStream.Close();
        }
    }
}
