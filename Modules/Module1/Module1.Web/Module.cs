using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Module1.Abstractions;
using Module1.Data;
using Module1.Services;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;
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
            serviceCollection.AddDbContext<PlatformDbContext>(builder =>
            {
                builder.UseSqlServer("Data Source=(local);Initial Catalog=VirtoCommerce3.0;Persist Security Info=True;User ID=virto;Password=virto;MultipleActiveResultSets=True;Connect Timeout=30", x => x.MigrationsAssembly("Module1.Data"));
                builder.ReplaceService<IModelCustomizer, PlatformDbContextCustomizer>();
            });

        }

        public void PostInitialize(IServiceProvider serviceProvider)
        {
            var settingsService = serviceProvider.GetRequiredService<ISettingsManager>();
            var platformRepository = serviceProvider.GetRequiredService<IPlatformRepository>();
            var dynamicPropertyRegistrar = serviceProvider.GetRequiredService<IDynamicPropertyRegistrar>();
            dynamicPropertyRegistrar.RegisterType<TestClass>();

            using (var platformDbContext = serviceProvider.GetRequiredService<PlatformDbContext>())
            {
                platformDbContext.Database.EnsureCreated();
                platformDbContext.Database.Migrate();
            }

        }

        public void Uninstall()
        {
        }
    }
}
