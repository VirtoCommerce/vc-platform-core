using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.PushNotifications;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Assets.AzureBlobStorage;
using VirtoCommerce.Platform.Data.Assets.FileSystem;
using VirtoCommerce.Platform.Data.DynamicProperties;
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
            services.AddSingleton<IEventPublisher, InProcessBus>();
            services.AddSingleton<IDynamicPropertyService, DynamicPropertyService>();
            services.AddSingleton<IDynamicPropertySearchService, DynamicPropertySearchService>();
            services.AddSingleton<IDynamicPropertyRegistrar, DynamicPropertyService>();
            return services;

        }

        public static void AddFileSystemBlobProvider(this IServiceCollection services, Action<FileSystemBlobContentOptions> setupAction = null)
        {
            services.AddSingleton<IBlobStorageProvider, FileSystemBlobProvider>();
            services.AddSingleton<IBlobUrlResolver, FileSystemBlobProvider>();
            if (setupAction != null)
            {
                services.Configure(setupAction);
            }
        }

        public static void AddAzureBlobProvider(this IServiceCollection services, Action<AzureBlobContentOptions> setupAction = null)
        {
            services.AddSingleton<IBlobStorageProvider, AzureBlobProvider>();
            services.AddSingleton<IBlobUrlResolver, AzureBlobProvider>();
            if (setupAction != null)
            {
                services.Configure(setupAction);
            }
        }


    }
}
