using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Notifications;
using VirtoCommerce.Platform.Core.PushNotifications;
using VirtoCommerce.Platform.Core.TransactionFileManager;
using VirtoCommerce.Platform.Data.Caching;
using VirtoCommerce.Platform.Data.ChangeLog;
using VirtoCommerce.Platform.Data.DynamicProperties;
using VirtoCommerce.Platform.Data.ExportImport;
using VirtoCommerce.Platform.Data.PushNotifications;
using VirtoCommerce.Platform.Data.Repositories;
using VirtoCommerce.Platform.Data.Settings;

namespace VirtoCommerce.Platform.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatformServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PlatformDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("VirtoCommerce")));
            services.AddTransient<IPlatformRepository, PlatformRepository>();
            services.AddTransient<Func<IPlatformRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetService<IPlatformRepository>());

            services.AddSettings();

            services.AddDynamicProperties();

            services.AddSingleton<IPushNotificationManager, PushNotificationManager>();

            var inProcessBus = new InProcessBus();
            services.AddSingleton<IHandlerRegistrar>(inProcessBus);
            services.AddSingleton<IEventPublisher>(inProcessBus);
            services.AddTransient<IChangeLogService, ChangeLogService>();
            services.AddTransient<IChangeLogSearchService, ChangeLogSearchService>();
            //Use MemoryCache decorator to use global platform cache settings
            services.AddSingleton<IPlatformMemoryCache, PlatformMemoryCache>();
            services.AddScoped<IPlatformExportImportManager, PlatformExportImportManager>();
            services.AddSingleton<ITransactionFileManager, TransactionFileManager.TransactionFileManager>();

            services.AddTransient<IEmailSender, DefaultEmailSender>();
            services.AddSingleton(js =>
            {
                var serv = js.GetService<IOptions<MvcJsonOptions>>();
                return JsonSerializer.Create(serv.Value.SerializerSettings);
            });

            return services;

        }
    }
}
