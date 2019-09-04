using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ContentModule.Azure;
using VirtoCommerce.ContentModule.Azure.Extensions;
using VirtoCommerce.ContentModule.Core;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ContentModule.Data.ExportImport;
using VirtoCommerce.ContentModule.Data.Repositories;
using VirtoCommerce.ContentModule.Data.Services;
using VirtoCommerce.ContentModule.FileSystem;
using VirtoCommerce.ContentModule.FileSystem.Extensions;
using VirtoCommerce.Platform.Assets.AzureBlobStorage;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Data.Extensions;

namespace VirtoCommerce.ContentModule.Web
{
    public class Module : IModule, IExportSupport, IImportSupport
    {
        private IApplicationBuilder _appBuilder;
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            var snapshot = serviceCollection.BuildServiceProvider();
            var configuration = snapshot.GetService<IConfiguration>();
            var hostingEnvironment = snapshot.GetService<IHostingEnvironment>();

            serviceCollection.AddDbContext<MenuDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("VirtoCommerce")));
            serviceCollection.AddTransient<IMenuRepository, MenuRepository>();
            serviceCollection.AddTransient<Func<IMenuRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetService<IMenuRepository>());

            serviceCollection.AddTransient<IMenuService, MenuService>();

            serviceCollection.AddTransient<ContentExportImport>();

            var contentProvider = configuration.GetSection("Content:Provider").Value;
            if (contentProvider.EqualsInvariant(AzureBlobProvider.ProviderName))
            {
                serviceCollection.AddOptions<AzureContentBlobOptions>().Bind(configuration.GetSection("Content:AzureBlobStorage")).ValidateDataAnnotations();
                serviceCollection.AddAzureContentBlobProvider();
            }
            else
            {
                serviceCollection.AddOptions<FileSystemContentBlobOptions>().Bind(configuration.GetSection("Content:FileSystem")).ValidateDataAnnotations();
                serviceCollection.AddFileSystemContentBlobProvider(options =>
                {
                    options.RootPath = hostingEnvironment.MapPath(options.RootPath);
                });
            }
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            _appBuilder = appBuilder;

            var dynamicPropertyRegistrar = appBuilder.ApplicationServices.GetRequiredService<IDynamicPropertyRegistrar>();
            dynamicPropertyRegistrar.RegisterType<FrontMatterHeaders>();

            //Register module permissions
            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsProvider.RegisterPermissions(ContentConstants.Security.Permissions.AllPermissions.Select(x =>
                new Permission()
                {
                    GroupName = "Content",
                    ModuleId = ModuleInfo.Id,
                    Name = x
                }).ToArray());

            //Force migrations
            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                using (var menuDbContext = serviceScope.ServiceProvider.GetRequiredService<MenuDbContext>())
                {
                    menuDbContext.Database.MigrateIfNotApplied(MigrationName.GetUpdateV2MigrationName(ModuleInfo.Id));
                    menuDbContext.Database.EnsureCreated();
                    menuDbContext.Database.Migrate();
                }
            }

            var dynamicPropertyService = appBuilder.ApplicationServices.GetRequiredService<IDynamicPropertyService>();
            dynamicPropertyService.SaveDynamicPropertiesAsync(FrontMatterHeaders.AllDynamicProperties.ToArray()).GetAwaiter().GetResult();
        }

        public void Uninstall()
        {
        }

        public async Task ExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            await _appBuilder.ApplicationServices.GetRequiredService<ContentExportImport>().DoExportAsync(outStream, options, progressCallback, cancellationToken);
        }

        public async Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            await _appBuilder.ApplicationServices.GetRequiredService<ContentExportImport>().DoImportAsync(inputStream, options, progressCallback, cancellationToken);
        }
    }
}
