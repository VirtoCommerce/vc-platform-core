using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Extensions;
using VirtoCommerce.SitemapsModule.Core;
using VirtoCommerce.SitemapsModule.Core.Services;
using VirtoCommerce.SitemapsModule.Data.ExportImport;
using VirtoCommerce.SitemapsModule.Data.Repositories;
using VirtoCommerce.SitemapsModule.Data.Services;
using VirtoCommerce.SitemapsModule.Data.Services.SitemapItemRecordProviders;
using VirtoCommerce.Tools;

namespace VirtoCommerce.SitemapsModule.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class Module : IModule, IExportSupport, IImportSupport
    {
        private IApplicationBuilder _appBuilder;

        /// <summary>
        /// 
        /// </summary>
        public ManifestModuleInfo ModuleInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceCollection"></param>
        public void Initialize(IServiceCollection serviceCollection)
        {
            var configuration = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("VirtoCommerce.Sitemaps") ??
                                   configuration.GetConnectionString("VirtoCommerce");
            serviceCollection.AddDbContext<SitemapDbContext>(options => options.UseSqlServer(connectionString));
            serviceCollection.AddTransient<ISitemapRepository, SitemapRepository>();
            serviceCollection.AddTransient<Func<ISitemapRepository>>(provider =>
                () => provider.CreateScope().ServiceProvider.GetRequiredService<ISitemapRepository>());

            serviceCollection.AddTransient<ISitemapService, SitemapService>();
            serviceCollection.AddTransient<ISitemapItemService, SitemapItemService>();
            serviceCollection.AddTransient<ISitemapSearchService, SitemapSearchService>();
            serviceCollection.AddTransient<ISitemapItemSearchService, SitemapItemSearchService>();
            serviceCollection.AddTransient<IUrlBuilder, UrlBuilder>();
            serviceCollection.AddTransient<ISitemapUrlBuilder, SitemapUrlBuilder>();
            serviceCollection.AddTransient<ISitemapItemRecordProvider, CatalogSitemapItemRecordProvider>();
            serviceCollection.AddTransient<ISitemapItemRecordProvider, CustomSitemapItemRecordProvider>();
            serviceCollection.AddTransient<ISitemapItemRecordProvider, VendorSitemapItemRecordProvider>();
            serviceCollection.AddTransient<ISitemapItemRecordProvider, StaticContentSitemapItemRecordProvider>();
            serviceCollection.AddTransient<ISitemapXmlGenerator, SitemapXmlGenerator>();
            serviceCollection.AddTransient<SitemapExportImport>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appBuilder"></param>
        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            _appBuilder = appBuilder;

            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<SitemapDbContext>();
                dbContext.Database.MigrateIfNotApplied(MigrationName.GetUpdateV2MigrationName(ModuleInfo.Id));
                dbContext.Database.EnsureCreated();
                dbContext.Database.Migrate();
            }

            var settingsRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);

            var allPermissions = ModuleConstants.Security.Permissions.AllPermissions.Select(permissionName => new Permission
            {
                Name = permissionName,
                GroupName = "Sitemaps",
                ModuleId = ModuleInfo.Id
            }).ToArray();
            var permissionsRegistrar = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsRegistrar.RegisterPermissions(allPermissions);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Uninstall()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outStream"></param>
        /// <param name="options"></param>
        /// <param name="progressCallback"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            await _appBuilder.ApplicationServices.GetRequiredService<SitemapExportImport>().DoExportAsync(outStream,
                progressCallback, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="options"></param>
        /// <param name="progressCallback"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            await _appBuilder.ApplicationServices.GetRequiredService<SitemapExportImport>().DoImportAsync(inputStream,
                progressCallback, cancellationToken);
        }
    }
}
