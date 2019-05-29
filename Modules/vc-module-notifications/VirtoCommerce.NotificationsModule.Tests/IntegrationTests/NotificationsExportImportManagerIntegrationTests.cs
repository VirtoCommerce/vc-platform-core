using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Data.ExportImport;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.NotificationsModule.Data.Services;
using VirtoCommerce.NotificationsModule.Web.JsonConverters;
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

            if (!AbstractTypeFactory<NotificationTemplate>.AllTypeInfos.SelectMany(x => x.AllSubclasses).Contains(typeof(EmailNotificationTemplate)))
                AbstractTypeFactory<NotificationTemplate>.RegisterType<EmailNotificationTemplate>().MapToType<NotificationTemplateEntity>();

            if (!AbstractTypeFactory<NotificationMessage>.AllTypeInfos.SelectMany(x => x.AllSubclasses).Contains(typeof(EmailNotificationMessage)))
                AbstractTypeFactory<NotificationMessage>.RegisterType<EmailNotificationMessage>().MapToType<NotificationMessageEntity>();

            _notificationRegistrar = new NotificationService(RepositoryFactory, _eventPulisherMock.Object);
            _notificationRegistrar.RegisterNotification<RegistrationEmailNotification>();
        }

        private JsonSerializer GetJsonSerializer()
        {
            return JsonSerializer.Create(new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter> { new NotificationPolymorphicJsonConverter() }
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

            entity.TenantIdentity = new TenantIdentity(Guid.NewGuid().ToString(), nameof(Notification));
            entity.Templates = new ObservableCollection<NotificationTemplate>()
            {
                new EmailNotificationTemplate() { Body = "test", LanguageCode = "en-US" },
            };

            _notificationSearchServiceMock
                .Setup(nss => nss.SearchNotificationsAsync(It.IsAny<NotificationSearchCriteria>()))
                .ReturnsAsync(new NotificationSearchResult { Results = new List<Notification> { entity }, TotalCount = 1 });

            //Act
            await _notificationsExportImportManager.DoExportAsync(fileStream, exportImportProgressInfo => { }, new CancellationTokenWrapper(CancellationToken.None));

            //Assert
            fileStream.Close();
        }

        [Fact]
        public async Task DoImport_SuccessImport()
        {
            //Arrange
            var fileStream = new FileStream(Path.GetFullPath("export_test.json"), FileMode.Open);

            //Act
            await _notificationsExportImportManager.DoImportAsync(fileStream, exportImportProgressInfo => { }, new CancellationTokenWrapper(CancellationToken.None));

            //Assert
            fileStream.Close();
        }
    }
}
