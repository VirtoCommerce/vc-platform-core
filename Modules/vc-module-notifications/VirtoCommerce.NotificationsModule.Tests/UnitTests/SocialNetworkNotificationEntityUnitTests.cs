using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.NotificationsModule.Data.Services;
using VirtoCommerce.NotificationsModule.Tests.NotificationTypes;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using Xunit;

namespace VirtoCommerce.NotificationsModule.Tests.UnitTests
{
    public class SocialNetworkNotificationEntity : NotificationEntity
    {
        [StringLength(128)]
        public string Token { get; set; }

        public override Notification ToModel(Notification notification)
        {
            var socialNetworkNotification = notification as SocialNetworkNotification;

            if (socialNetworkNotification != null)
            {
                socialNetworkNotification.Token = this.Token;
            }

            return base.ToModel(notification);
        }

        public override NotificationEntity FromModel(Notification notification, PrimaryKeyResolvingMap pkMap)
        {
            var socialNetworkNotification = notification as SocialNetworkNotification;
            if (socialNetworkNotification != null)
            {
                this.Token = socialNetworkNotification.Token;
                
            }

            return base.FromModel(notification, pkMap);
        }

        public override void Patch(NotificationEntity notification)
        {
            var socialNetworkNotification = notification as SocialNetworkNotificationEntity;
            if (socialNetworkNotification != null)
            {
                socialNetworkNotification.Token = this.Token;
            }

            base.Patch(notification);
        }
    }

    public class SocialNetworkNotification : Notification
    {
        [StringLength(128)]
        public string Token { get; set; }
    }
    public class SocialNetworkTemplate : NotificationTemplate { }
    public class SocialNetworkMessage : NotificationMessage { }
    public class RegistrationSocialNetworkNotification : SocialNetworkNotification { }

    public class SocialNetworkNotificationEntityUnitTests
    {
        private readonly Mock<INotificationRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly INotificationRegistrar _notificationRegistrar;
        private readonly Func<INotificationRepository> _repositoryFactory;
        private readonly Mock<IEventPublisher> _eventPublisherMock;
        private readonly NotificationService _notificationService;

        public SocialNetworkNotificationEntityUnitTests()
        {
            _repositoryMock = new Mock<INotificationRepository>();
            _repositoryFactory = () => _repositoryMock.Object;
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _repositoryMock.Setup(ss => ss.UnitOfWork).Returns(_mockUnitOfWork.Object);
            _eventPublisherMock = new Mock<IEventPublisher>();
            _notificationService = new NotificationService(_repositoryFactory, _eventPublisherMock.Object);
            _notificationRegistrar = _notificationService;
            //todo
            if (!AbstractTypeFactory<Notification>.AllTypeInfos.Any(t => t.IsAssignableTo(nameof(SocialNetworkNotification))))
            {
                AbstractTypeFactory<Notification>.RegisterType<SocialNetworkNotification>().MapToType<NotificationEntity>();
            }

            if (!AbstractTypeFactory<NotificationTemplate>.AllTypeInfos.Any(t => t.IsAssignableTo(nameof(SocialNetworkTemplate))))
            {
                AbstractTypeFactory<NotificationTemplate>.RegisterType<SocialNetworkTemplate>().MapToType<NotificationTemplateEntity>();
            }

            if (!AbstractTypeFactory<NotificationMessage>.AllTypeInfos.Any(t => t.IsAssignableTo(nameof(SocialNetworkMessage))))
            {
                AbstractTypeFactory<NotificationMessage>.RegisterType<SocialNetworkMessage>().MapToType<NotificationMessageEntity>();
            }

            if (!AbstractTypeFactory<NotificationEntity>.AllTypeInfos.Any(t => t.IsAssignableTo(nameof(SocialNetworkNotificationEntity))))
            {
                AbstractTypeFactory<NotificationEntity>.RegisterType<SocialNetworkNotificationEntity>();
            }
        }

        [Fact]
        public async Task GetNotificationByTypeAsync_ReturnNotifiction()
        {
            //Arrange
            string type = nameof(RegistrationSocialNetworkNotification);
            _repositoryMock.Setup(n => n.GetByTypeAsync(nameof(RegistrationSocialNetworkNotification), null, null))
                .ReturnsAsync(new SocialNetworkNotificationEntity() { IsActive = true });
            _notificationRegistrar.RegisterNotification<RegistrationSocialNetworkNotification>();

            //Act
            var result = await _notificationService.GetByTypeAsync(type, null, null);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(type, result.Type);
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task GetNotificationsByIdsAsync_ReturnNotifications()
        {
            //Arrange
            string id = Guid.NewGuid().ToString();
            var notifications = new List<NotificationEntity> { new SocialNetworkNotificationEntity() { Id = id, Type = nameof(SocialNetworkNotification) } };
            _repositoryMock.Setup(n => n.GetByIdsAsync(new[] { id })).ReturnsAsync(notifications.ToArray());
            _notificationRegistrar.RegisterNotification<RegistrationSocialNetworkNotification>();

            //Act
            var result = await _notificationService.GetByIdsAsync(new[] { id });

            //Assert
            Assert.NotNull(result);
            Assert.Contains(result, r => r.Id.Equals(id));
        }

        [Fact]
        public async Task SaveChangesAsync_SavedNotification()
        {
            //Arrange
            string id = Guid.NewGuid().ToString();
            _notificationRegistrar.RegisterNotification<RegistrationSocialNetworkNotification>();
            var notificationEntities = new List<NotificationEntity>
            {
                new SocialNetworkNotificationEntity()
                {
                    Id = id,
                    Type = nameof(RegistrationSocialNetworkNotification),
                    Kind = nameof(SocialNetworkNotification)
                }
            };
            _repositoryMock.Setup(n => n.GetByIdsAsync(new[] { id }))
                .ReturnsAsync(notificationEntities.ToArray());
            var notifications = notificationEntities.Select(n => n.ToModel(AbstractTypeFactory<Notification>.TryCreateInstance(n.Type)));

            //Act
            await _notificationService.SaveChangesAsync(notifications.ToArray());
        }
    }

    
}
