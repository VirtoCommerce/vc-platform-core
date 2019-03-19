using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.CoreModule.Core;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.CoreModule.Core.Conditions.Browse;
using VirtoCommerce.CoreModule.Core.Conditions.GeoConditions;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.CoreModule.Core.Package;
using VirtoCommerce.CoreModule.Core.Payment;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.CoreModule.Core.Shipping;
using VirtoCommerce.CoreModule.Core.Tax;
using VirtoCommerce.CoreModule.Data.Currency;
using VirtoCommerce.CoreModule.Data.Package;
using VirtoCommerce.CoreModule.Data.Registrars;
using VirtoCommerce.CoreModule.Data.Repositories;
using VirtoCommerce.CoreModule.Data.Seo;
using VirtoCommerce.CoreModule.Data.Services;
using VirtoCommerce.CoreModule.Data.Shipping;
using VirtoCommerce.CoreModule.Web.ExportImport;
using VirtoCommerce.CoreModule.Web.JsonConverters;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.CoreModule.Web
{
    public class Module : IModule, IExportSupport, IImportSupport
    {
        public ManifestModuleInfo ModuleInfo { get; set; }
        private IApplicationBuilder _appBuilder;

        public void Initialize(IServiceCollection serviceCollection)
        {
            var configuration = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
            serviceCollection.AddTransient<ICoreRepository, CoreRepositoryImpl>();
            var connectionString = configuration.GetConnectionString("VirtoCommerce.Core") ?? configuration.GetConnectionString("VirtoCommerce");
            serviceCollection.AddDbContext<CoreDbContext>(options => options.UseSqlServer(connectionString));
            serviceCollection.AddSingleton<Func<ICoreRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<ICoreRepository>());
            serviceCollection.AddSingleton<ISeoService, SeoService>();
            serviceCollection.AddSingleton<ICurrencyService, CurrencyService>();
            serviceCollection.AddSingleton<IPackageTypesService, PackageTypesService>();
            //Can be overrided
            serviceCollection.AddSingleton<ISeoDuplicatesDetector, NullSeoDuplicateDetector>();
            serviceCollection.AddSingleton<IShippingMethodsRegistrar>(new ShippingMethodRegistrar());
            serviceCollection.AddSingleton<IPaymentMethodsRegistrar>(new PaymentMethodsRegistrar());
            serviceCollection.AddSingleton<ITaxProviderRegistrar>(new TaxProviderRegistrar());
            serviceCollection.AddSingleton<CoreExportImport>();
            serviceCollection.AddSingleton<IUniqueNumberGenerator, SequenceUniqueNumberGeneratorService>();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            _appBuilder = appBuilder;
            var settingsRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);


            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsProvider.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions.Select(x => new Permission() { GroupName = "Core", Name = x }).ToArray());

            var mvcJsonOptions = appBuilder.ApplicationServices.GetService<IOptions<MvcJsonOptions>>();
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(new PolymorphicJsonConverter());
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(new ConditionJsonConverter());

            AbstractTypeFactory<IConditionTree>.RegisterType<BlockConditionAndOr>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionAgeIs>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionGenderIs>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionLanguageIs>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionStoreSearchedPhrase>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionUrlIs>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionGeoCity>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionGeoCountry>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionGeoState>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionGeoTimeZone>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionGeoZipCode>();
            AbstractTypeFactory<IConditionTree>.RegisterType<UserGroupsContainsCondition>();

            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<CoreDbContext>();
                dbContext.Database.EnsureCreated();
                dbContext.Database.Migrate();
            }

        }

        public void Uninstall()
        {
        }

        public Task ExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            return _appBuilder.ApplicationServices.GetRequiredService<CoreExportImport>().ExportAsync(outStream, options, progressCallback, cancellationToken);
        }

        public Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            return _appBuilder.ApplicationServices.GetRequiredService<CoreExportImport>().ImportAsync(inputStream, options, progressCallback, cancellationToken);
        }
    }
}

