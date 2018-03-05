using Moq;
using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.NotificationsModule.Data.Services;
using Xunit;

namespace VirtoCommerce.NotificationsModule.Tests.UnitTests
{
    public class NotificationServiceUnitTests
    {
        private readonly Mock<INotificationRepository> _repositoryMock;
        private readonly INotificationRegistrar _notificationRegistrar;
        private readonly NotificationService _notificationSearchService;

        public NotificationServiceUnitTests()
        {

        }

        [Fact]
        public void GetNotificationByTypeAsync_ReturnNotifictions()
        {

        }

        [Fact]
        public void GetNotificationsByIdsAsync_ReturnNotifications()
        {

        }

        [Fact]
        public void SaveChangesAsync_SavedNotification()
        {

        }

        [Fact]
        public void RegisterNotification_ReisteredNotification()
        {

        }

    }
}
