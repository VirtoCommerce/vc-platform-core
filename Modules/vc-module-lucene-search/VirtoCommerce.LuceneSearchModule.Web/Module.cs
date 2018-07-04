using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.LuceneSearchModule.Data;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SearchModule.Core;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.LuceneSearchModule.Web
{
    public class Module : IModule
    {

        public ManifestModuleInfo ModuleInfo { get; set; }
        public void Initialize(IServiceCollection serviceCollection)
        {
            var snapshot = serviceCollection.BuildServiceProvider();
            var configuration = snapshot.GetService<IConfiguration>();
            var settingsManager = snapshot.GetService<ISettingsManager>();

            var searchConnectionString = configuration.GetValue<string>("Search:Lucene:SearchConnectionString");

            if (string.IsNullOrEmpty(searchConnectionString))
            {
                searchConnectionString = settingsManager.GetValue(ModuleConstants.Settings.General.SearchConnectionString.Name, string.Empty);
            }

            var searchConnection = new SearchConnection(searchConnectionString);

            if (!string.IsNullOrEmpty(searchConnectionString))
            {
                serviceCollection.AddSingleton<ISearchConnection>(searchConnection);
            }

            var dataDirectoryPath = Path.GetFullPath(searchConnection["DataDirectoryPath"] ?? searchConnection["server"]);
            var luceneSettings = new LuceneSearchProviderSettings(dataDirectoryPath, searchConnection.Scope);

            serviceCollection.AddSingleton(luceneSettings);
            serviceCollection.AddSingleton<ISearchProvider, LuceneSearchProvider>();
        }

        public void PostInitialize(IApplicationBuilder serviceProvider)
        {
        }

        public void Uninstall()
        {
        }
    }
}
