using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
            var configuration = serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>();
            //var mode = FluentValidation.CascadeMode.Continue;
            serviceCollection.AddSingleton<IMyService, MyServiceImpl>();
            serviceCollection.AddDbContext<PlatformDbContext2>(builder =>
            {
                builder.UseSqlServer(configuration.GetConnectionString("VirtoCommerce"));
            });
            serviceCollection.AddTransient<IPlatformRepository, PlatformRepository2>();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            var dynamicPropertyRegistrar = appBuilder.ApplicationServices.GetRequiredService<IDynamicPropertyRegistrar>();
            dynamicPropertyRegistrar.RegisterType<TestClass>();

            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                using (var platformDbContext = serviceScope.ServiceProvider.GetRequiredService<PlatformDbContext2>())
                {
                    platformDbContext.Database.EnsureCreated();
                    platformDbContext.Database.Migrate();
                }
            }
            //AbstractTypeFactory<SettingEntity>.OverrideType<SettingEntity, SettingEntity2>();
            //AbstractTypeFactory<SettingEntry>.OverrideType<SettingEntry, SettingEntry2>();
        }

        public void Uninstall()
        {
        }
    }
}
