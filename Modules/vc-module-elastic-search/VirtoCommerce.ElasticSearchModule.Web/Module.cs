using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ElasticSearchModule.Data;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.ElasticSearchModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }
        public void Initialize(IServiceCollection serviceCollection)
        {
            var snapshot = serviceCollection.BuildServiceProvider();
            var configuration = snapshot.GetService<IConfiguration>();
            var searchProvider = configuration.GetValue<string>("Search:Provider");

            if (searchProvider.EqualsInvariant("Elasticsearch"))
            {
                var searchConnectionString = configuration.GetValue<string>("Search:Elasticsearch:SearchConnectionString");
                var searchConnection = new SearchConnection(searchConnectionString, searchProvider);
                serviceCollection.AddSingleton<ISearchConnection>(searchConnection);
                serviceCollection.AddSingleton<ISearchProvider, ElasticSearchProvider>();
            }
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            var settingsRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);
        }

        public void Uninstall()
        {
            // not needed
        }
    }
}
