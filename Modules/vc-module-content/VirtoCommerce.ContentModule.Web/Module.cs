using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.ContentModule.Core;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ContentModule.Data.ExportImport;
using VirtoCommerce.ContentModule.Data.Repositories;
using VirtoCommerce.ContentModule.Data.Services;
using VirtoCommerce.ContentModule.Web.Infrastructure;
using VirtoCommerce.ContentModule.Web.Model;
using VirtoCommerce.Platform.Assets.AzureBlobStorage;
using VirtoCommerce.Platform.Assets.FileSystem;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Web.Extensions;

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
            serviceCollection.AddSingleton<Func<IMenuRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetService<IMenuRepository>());

            serviceCollection.AddSingleton<IMenuService, MenuService>();

            serviceCollection.AddSingleton<ContentExportImport>();

            var contentProvider = configuration.GetSection("Content:Provider").Value;


            if (contentProvider.EqualsInvariant(AzureBlobProvider.ProviderName))
            {
                var azureBlobContentOptions = new AzureBlobContentOptions();
                configuration.GetSection("Content:AzureBlobStorage").Bind(azureBlobContentOptions);

                //avoid closure
                var rootPath = $"{azureBlobContentOptions.RootPath}";
                var options = Options.Create(azureBlobContentOptions);

                serviceCollection.AddTransient<AzureContentBlobStorageProvider>().Configure<AzureContentBlobStorageProvider>(configuration.GetSection("Content:AzureBlobStorage"));

                serviceCollection.AddTransient<Func<string, IContentStorageProviderFactory>>(provider => (path) =>
                {
                    options.Value.RootPath = Path.Combine(rootPath, path);

                    return new AzureContentBlobStorageProvider(options);
                });
            }
            else
            {
                var fileSystemContentBlobOptions = new FileSystemBlobContentOptions();
                configuration.GetSection("Content:LocalStorage").Bind(fileSystemContentBlobOptions);

                fileSystemContentBlobOptions.RootPath = hostingEnvironment.MapPath(fileSystemContentBlobOptions.RootPath);

                //avoid closure
                var rootPath = $"{fileSystemContentBlobOptions.RootPath}";
                var options = Options.Create(fileSystemContentBlobOptions);


                serviceCollection.AddSingleton<FileSystemContentBlobStorageProvider>().Configure<FileSystemBlobContentOptions>(configuration.GetSection("Content:LocalStorage"));

                serviceCollection.AddSingleton<Func<string, IContentStorageProviderFactory>>(provider => (path) =>
                {
                    var urlHelper = provider.GetService<IUrlHelper>();
                    options.Value.RootPath = Path.Combine(rootPath, path.Replace("/", "\\"));

                    return new FileSystemContentBlobStorageProvider(options, urlHelper);
                });
            }

        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            _appBuilder = appBuilder;

            var dynamicPropertyRegistrar = appBuilder.ApplicationServices.GetRequiredService<IDynamicPropertyRegistrar>();
            dynamicPropertyRegistrar.RegisterType<FrontMatterHeaders>();

            var dynamicPropertyService = appBuilder.ApplicationServices.GetRequiredService<IDynamicPropertyService>();
            dynamicPropertyService.SaveDynamicPropertiesAsync(DynamicProperties.AllDynamicProperties.ToArray()).GetAwaiter();

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
                    menuDbContext.Database.EnsureCreated();
                    menuDbContext.Database.Migrate();
                }
            }
        }

        public void Uninstall()
        {
        }

        public Task ExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            return _appBuilder.ApplicationServices.GetRequiredService<ContentExportImport>().DoExportAsync(outStream, options, progressCallback, cancellationToken);
        }

        public Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            return _appBuilder.ApplicationServices.GetRequiredService<ContentExportImport>().ImportAsync(inputStream, options, progressCallback, cancellationToken);
        }

    }
}
