using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Module1.Abstractions;
using Module1.Data;
using Module1.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Model;
using VirtoCommerce.Platform.Data.Repositories;

namespace Module1.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            //var mode = FluentValidation.CascadeMode.Continue;
            serviceCollection.AddSingleton<IMyService, MyServiceImpl>();
            serviceCollection.AddDbContext<PlatformDbContext2>(builder =>
            {
                builder.UseSqlServer("Data Source=(local);Initial Catalog=VirtoCommerce3.0;Persist Security Info=True;User ID=virto;Password=virto;MultipleActiveResultSets=True;Connect Timeout=30");
            });
            serviceCollection.AddTransient<IPlatformRepository, PlatformRepository2>();
        }

        public void PostInitialize(IServiceProvider serviceProvider)
        {
            var settingsService = serviceProvider.GetRequiredService<ISettingsManager>();
            var platformRepository = serviceProvider.GetRequiredService<IPlatformRepository>();
            var dynamicPropertyRegistrar = serviceProvider.GetRequiredService<IDynamicPropertyRegistrar>();
            dynamicPropertyRegistrar.RegisterType<TestClass>();

            using (var platformDbContext = serviceProvider.GetRequiredService<PlatformDbContext2>())
            {
                platformDbContext.Database.EnsureCreated();
                platformDbContext.Database.Migrate();
            }
            AbstractTypeFactory<SettingEntity>.OverrideType<SettingEntity, SettingEntity2>();
            AbstractTypeFactory<SettingEntry>.OverrideType<SettingEntry, SettingEntry2>();
        }

        public void Uninstall()
        {
        }
    }
}
