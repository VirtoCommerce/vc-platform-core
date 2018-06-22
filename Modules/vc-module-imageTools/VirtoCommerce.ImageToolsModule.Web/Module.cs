using System;
using System.IO;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ImageToolsModule.Core.Models;
using VirtoCommerce.ImageToolsModule.Core.Services;
using VirtoCommerce.ImageToolsModule.Data.Models;
using VirtoCommerce.ImageToolsModule.Data.Repositories;
using VirtoCommerce.ImageToolsModule.Data.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.PushNotifications;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Data.PushNotifications;
using VirtoCommerce.Platform.Security;

namespace VirtoCommerce.ImageToolsModule.Web
{
    public class Module : IModule, ISupportExportImportModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }
        public void Initialize(IServiceCollection serviceCollection)
        {
            var snapshot = serviceCollection.BuildServiceProvider();
            var configuration = snapshot.GetService<IConfiguration>();

            serviceCollection.AddDbContext<ThumbnailDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("VirtoCommerce")));

            serviceCollection.AddTransient<IThumbnailRepository, ThumbnailRepository>();
            serviceCollection.AddSingleton<Func<IThumbnailRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetService<IThumbnailRepository>());


            serviceCollection.AddSingleton<IThumbnailOptionService, ThumbnailOptionService>();
            serviceCollection.AddSingleton<IThumbnailOptionSearchService, ThumbnailOptionSearchService>();

            serviceCollection.AddSingleton<IThumbnailTaskSearchService, ThumbnailTaskSearchService>();
            serviceCollection.AddSingleton<IThumbnailTaskService, ThumbnailTaskService>();
            serviceCollection.AddSingleton<IPushNotificationManager, PushNotificationManager>();
        }

        public void PostInitialize(IServiceProvider serviceProvider)
        {
            AbstractTypeFactory<ThumbnailOption>.RegisterType<ThumbnailOption>().MapToType<ThumbnailOptionEntity>();
            AbstractTypeFactory<ThumbnailTask>.RegisterType<ThumbnailTask>().MapToType<ThumbnailTaskEntity>();

            using (var thumbnailDbContext = serviceProvider.GetRequiredService<ThumbnailDbContext>())
            {
                thumbnailDbContext.Database.EnsureCreated();
                thumbnailDbContext.Database.Migrate();
            }
        }

        public void Uninstall()
        {
            throw new NotImplementedException();
        }

        #region ISupportExportImportModule Members

        public void DoExport(Stream outStream, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void DoImport(Stream inputStream, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
