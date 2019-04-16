using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.NotificationsModule.Data.Model;
using VirtoCommerce.NotificationsModule.Data.Repositories;
using VirtoCommerce.NotificationsModule.Data.Services;
using VirtoCommerce.NotificationsSampleModule.Web.Models;
using VirtoCommerce.NotificationsSampleModule.Web.Repositories;
using VirtoCommerce.NotificationsSampleModule.Web.Types;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using Xunit;

namespace VirtoCommerce.NotificationsModule.Tests.IntegrationTests
{
    public class TwitterNotificationServiceIntegrationTest
    {
        private readonly TwitterNotificationDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly INotificationSearchService _notificationSearchService;

        public TwitterNotificationServiceIntegrationTest()
        {
            var container = new ServiceCollection();
            container.AddDbContext<TwitterNotificationDbContext>(options => options.UseSqlServer("Data Source=(local);Initial Catalog=VirtoCommerce3;Persist Security Info=True;User ID=virto;Password=virto;MultipleActiveResultSets=True;Connect Timeout=30"));
            container.AddScoped<INotificationRepository, TwitterNotificationRepository>();
            container.AddScoped<INotificationService, NotificationService>();
            container.AddScoped<Func<INotificationRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetService<INotificationRepository>());
            container.AddScoped<IEventPublisher, InProcessBus>();
            container.AddScoped<INotificationRegistrar, NotificationService>();
            container.AddScoped<INotificationSearchService, NotificationSearchService>();

            var serviceProvider = container.BuildServiceProvider();
            _notificationService = serviceProvider.GetService<INotificationService>();
            _notificationSearchService = serviceProvider.GetService<INotificationSearchService>();

            if (AbstractTypeFactory<NotificationEntity>.AllTypeInfos.All(t => t.Type != typeof(TwitterNotificationEntity)))
            {
                AbstractTypeFactory<NotificationEntity>.RegisterType<TwitterNotificationEntity>();
            }

            var registrar = serviceProvider.GetService<INotificationRegistrar>();
            registrar.RegisterNotification<TwitterNotification>();
            registrar.RegisterNotification<TwitterNotification, NotificationEntity>();
            registrar.RegisterNotification<PostTwitterNotification>();
        }

        [Fact]
        public async Task SaveChangesAsync_CreateTwitterNotification()
        {
            //Arrange
            var notifications = new List<TwitterNotification>()
            {
                new TwitterNotification
                {
                    Type = nameof(PostTwitterNotification), IsActive = true,
                    TenantIdentity = new TenantIdentity("Platform", null),
                    Post = $"Post {DateTime.Now}"
                }
            };

            //Act
            await _notificationService.SaveChangesAsync(notifications.ToArray());

            //Assert
        }

        [Fact]
        public async Task SearchNotifications_ContainsTwitterNotification()
        {
            //Arrange
            var criteria = new NotificationSearchCriteria() { Take = int.MaxValue };

            //Act
            var result = await _notificationSearchService.SearchNotificationsAsync(criteria);

            //Assert
            Assert.Contains(result.Results, n => n.Type == nameof(PostTwitterNotification) && n.IsActive);
        }

        [Fact]
        public async Task SaveChangesAsync_UpdateTwitterNotification()
        {
            //Arrange
            var criteria = new NotificationSearchCriteria() { Take = int.MaxValue };
            var notifications = await _notificationSearchService.SearchNotificationsAsync(criteria);
            var notification = notifications.Results.FirstOrDefault();
            if (notification is TwitterNotification twitterNotification)
            {
                twitterNotification.Post = $"Post {DateTime.Now}";
            }

            //Act
            await _notificationService.SaveChangesAsync(new[] { notification });

            //Assert
        }
    }
}
