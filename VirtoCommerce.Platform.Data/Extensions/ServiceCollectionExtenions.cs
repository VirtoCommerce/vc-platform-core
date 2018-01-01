using System;
using System.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;
using VirtoCommerce.Platform.Data.Repositories;
using VirtoCommerce.Platform.Data.Repositories.Migrations;
using VirtoCommerce.Platform.Data.Settings;

namespace VirtoCommerce.Platform.Data.Extensions
{
    public static class ServiceCollectionExtenions
    {
        public static IServiceCollection AddPlatformServices(this IServiceCollection services, IConfiguration configuration)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<PlatformRepository, Configuration>());

            services.AddScoped<AuditableInterceptor>();
            services.AddSingleton(provider => new Func<IPlatformRepository>(() => new PlatformRepository(configuration.GetConnectionString("VirtoCommerce"), new IInterceptor [] { provider.GetService<AuditableInterceptor>(), new EntityPrimaryKeyGeneratorInterceptor() })));
            services.AddTransient<IPlatformRepository, PlatformRepository>();
            services.AddSingleton<Func<IPlatformRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetService<IPlatformRepository>());
            services.AddSingleton<ISettingsManager, SettingsManager>();

            return services;

        }
    }
}
