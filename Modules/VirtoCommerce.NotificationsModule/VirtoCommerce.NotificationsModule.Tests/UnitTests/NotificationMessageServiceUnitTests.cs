using Moq;
using VirtoCommerce.NotificationsModule.Core.Abstractions;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.NotificationsModule.Data.Services;
using Xunit;

namespace VirtoCommerce.NotificationsModule.Tests.UnitTests
{
    public class NotificationMessageServiceUnitTests
    {
        private readonly Mock<INotificationRepository> _repositoryMock;
        private readonly INotificationRegistrar _notificationRegistrar;
        private readonly NotificationService _notificationSearchService;

        public NotificationMessageServiceUnitTests()
        {

        }

        [Fact]
        public void GetNotificationsMessageByIds_GetMessage()
        {

        }

        [Fact]
        public void SaveNotificationMessages_SaveMessage()
        {

        }
    }
}
