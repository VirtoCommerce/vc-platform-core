using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.ContentModule.Core;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ContentModule.Data.Extensions;
using VirtoCommerce.ContentModule.Data.Repositories;
using VirtoCommerce.ContentModule.Data.Services;
using VirtoCommerce.Platform.Assets.AzureBlobStorage;
using VirtoCommerce.Platform.Assets.FileSystem;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ContentModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            var snapshot = serviceCollection.BuildServiceProvider();
            var configuration = snapshot.GetService<IConfiguration>();
            serviceCollection.AddDbContext<MenuDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("VirtoCommerce")));
            serviceCollection.AddTransient<IMenuRepository, MenuRepository>();
            serviceCollection.AddSingleton<Func<IMenuRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetService<IMenuRepository>());

            serviceCollection.AddSingleton<IMenuService, MenuService>();

            var contentProvider = configuration.GetSection("Content:Provider").Value;


            if (contentProvider.EqualsInvariant(AzureBlobProvider.ProviderName))
            {
                serviceCollection.Configure<AzureBlobContentOptions>(configuration.GetSection("Content:AzureBlobStorage"));
                serviceCollection.AddTransient<IContentBlobStorageProvider, AzureContentBlobStorageProvider>();
            }
            else
            {
                var fileSystemContentBlobOptions = new FileSystemBlobContentOptions();
                configuration.GetSection("Content:LocalStorage").Bind(fileSystemContentBlobOptions);

                IOptions<FileSystemBlobContentOptions> options = Options.Create(fileSystemContentBlobOptions);

                serviceCollection.AddTransient<FileSystemContentBlobStorageProvider>().Configure<FileSystemBlobContentOptions>(configuration.GetSection("Content:LocalStorage"));

                serviceCollection.AddTransient<Func<string, IContentBlobStorageProvider>>(provider => (str) =>
                {
                    return new FileSystemContentBlobStorageProvider(options);
                });
            }

        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {

            //Register module permissions
            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IKnownPermissionsProvider>();
            permissionsProvider.RegisterPermissions(ContentConstants.Security.Permissions.AllPermissions.Select(x => new Permission() { GroupName = "Content", ModuleId = ModuleInfo.Id, Name = x }).ToArray());

        }

        public void Uninstall()
        {
        }
    }
}
