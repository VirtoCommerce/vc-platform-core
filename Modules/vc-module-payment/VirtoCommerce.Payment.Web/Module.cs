using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.PaymentModule.Core;
using VirtoCommerce.PaymentModule.Core.Services;
using VirtoCommerce.PaymentModule.Data;
using VirtoCommerce.PaymentModule.Data.Repositories;
using VirtoCommerce.PaymentModule.Data.Services;
using VirtoCommerce.PaymentModule.Web.JsonConverters;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Extensions;

namespace VirtoCommerce.PaymentModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            var snapshot = serviceCollection.BuildServiceProvider();
            var configuration = snapshot.GetService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("VirtoCommerce.Payment") ?? configuration.GetConnectionString("VirtoCommerce");
            serviceCollection.AddDbContext<PaymentDbContext>(options => options.UseSqlServer(connectionString));
            serviceCollection.AddTransient<IPaymentRepository, PaymentRepository>();
            serviceCollection.AddSingleton<Func<IPaymentRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetService<IPaymentRepository>());

            serviceCollection.AddSingleton<IPaymentMethodsService, PaymentMethodsService>();
            serviceCollection.AddSingleton<IPaymentMethodsRegistrar, PaymentMethodsService>();
            serviceCollection.AddSingleton<IPaymentMethodsSearchService, PaymentMethodsSearchService>();
        }

        public void PostInitialize(IApplicationBuilder applicationBuilder)
        {
            var settingsRegistrar = applicationBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);

            var paymentMethodsRegistrar = applicationBuilder.ApplicationServices.GetRequiredService<IPaymentMethodsRegistrar>();
            paymentMethodsRegistrar.RegisterPaymentMethod<DefaultManualPaymentMethod>();
            settingsRegistrar.RegisterSettingsForType(Core.ModuleConstants.Settings.DefaultManualPaymentMethod.AllSettings, typeof(DefaultManualPaymentMethod).Name);

            var mvcJsonOptions = applicationBuilder.ApplicationServices.GetService<IOptions<MvcJsonOptions>>();
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(new PolymorphicJsonConverter(paymentMethodsRegistrar));

            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<PaymentDbContext>();
                dbContext.Database.MigrateIfNotApplied(MigrationName.GetUpdateV2MigrationName(ModuleInfo.Id));
                dbContext.Database.EnsureCreated();
                dbContext.Database.Migrate();
            }
        }

        public void Uninstall()
        {
        }
    }
}
