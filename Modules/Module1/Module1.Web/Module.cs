using System;
using Microsoft.Extensions.DependencyInjection;
using Module1.Abstractions;
using Module1.Services;
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
            serviceCollection.AddSingleton<IMyService, MyServiceImpl>();
        }

        public void PostInitialize(IServiceProvider serviceProvider)
        {
            var settingsService = serviceProvider.GetRequiredService<ISettingsManager>();
            var platformRepository = serviceProvider.GetRequiredService<IPlatformRepository>();
            settingsService.SaveSettings(new SettingEntry[] { new SettingEntry { Name = "a222aaa", Title = "a22aaa", ValueType = SettingValueType.ShortText, Value = "ss" } });
        }

        public void Uninstall()
        {
        }
    }
}
