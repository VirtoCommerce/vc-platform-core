using System;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core.PushNotifications;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.PushNotifications;
using VirtoCommerce.Platform.Data.Repositories;
using VirtoCommerce.Platform.Data.Settings;

namespace VirtoCommerce.Platform.Data.Extensions
{
    public static class ServiceCollectionExtenions
    {
        public static IServiceCollection AddPlatformServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PlatformDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("VirtoCommerce")));
            services.AddTransient<IPlatformRepository, PlatformRepository>();
            services.AddSingleton<Func<IPlatformRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetService<IPlatformRepository>());
            services.AddSingleton<ISettingsManager, SettingsManager>();
            services.AddSingleton<IPushNotificationManager, PushNotificationManager>();
            return services;

        }
    }
}
