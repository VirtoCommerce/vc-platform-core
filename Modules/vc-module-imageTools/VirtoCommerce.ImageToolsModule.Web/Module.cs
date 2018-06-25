using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ImageToolsModule.Core;
using VirtoCommerce.ImageToolsModule.Core.Models;
using VirtoCommerce.ImageToolsModule.Core.Services;
using VirtoCommerce.ImageToolsModule.Core.ThumbnailGeneration;
using VirtoCommerce.ImageToolsModule.Data.Models;
using VirtoCommerce.ImageToolsModule.Data.Repositories;
using VirtoCommerce.ImageToolsModule.Data.Services;
using VirtoCommerce.ImageToolsModule.Data.ThumbnailGeneration;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;

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

            serviceCollection.AddSingleton<IImageResizer, ImageResizer>();
            serviceCollection.AddSingleton<IImageService, ImageService>();
            serviceCollection.AddSingleton<IThumbnailGenerator, DefaultThumbnailGenerator>();
            serviceCollection.AddSingleton<IThumbnailGenerationProcessor, ThumbnailGenerationProcessor>();
            serviceCollection.AddSingleton<IImagesChangesProvider, BlobImagesChangesProvider>();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            AbstractTypeFactory<ThumbnailOption>.RegisterType<ThumbnailOption>().MapToType<ThumbnailOptionEntity>();
            AbstractTypeFactory<ThumbnailTask>.RegisterType<ThumbnailTask>().MapToType<ThumbnailTaskEntity>();


            //Register module settings
            ModuleInfo.Settings.Add(new ModuleSettingsGroup
            {
                Name = "Thumbnail|General",
                Settings = ThumbnailConstants.Settings.General.AllSettings.ToArray()
            });

            //Register module permissions
            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IKnownPermissionsProvider>();
            permissionsProvider.RegisterPermissions(ThumbnailConstants.Permissions.Security.AllPermissions.Select(x => new Permission() { GroupName = "Thumbnail", Name = x }).ToArray());

            //Force migrations
            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                var thumbnailDbContext = serviceScope.ServiceProvider.GetRequiredService<ThumbnailDbContext>();
                thumbnailDbContext.Database.EnsureCreated();
                thumbnailDbContext.Database.Migrate();
            }
        }

        public void Uninstall()
        {
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
