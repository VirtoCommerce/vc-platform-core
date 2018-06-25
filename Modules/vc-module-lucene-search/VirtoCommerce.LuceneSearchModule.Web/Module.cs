using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VirtoCommerce.LuceneSearchModule.Data;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.LuceneSearchModule.Web
{
    public class Module : IModule
    {

        public ManifestModuleInfo ModuleInfo { get; set; }
        public void Initialize(IServiceCollection serviceCollection)
        {
            var snapshot = serviceCollection.BuildServiceProvider();
            var searchConnection = snapshot.GetService<ISearchConnection>();

            if (searchConnection?.Provider?.EqualsInvariant("Lucene") == true)
            {
                var env = snapshot.GetService<IHostingEnvironment>();
                var dataDirectoryPath = Path.GetFullPath(searchConnection["DataDirectoryPath"] ?? searchConnection["server"]);
                var luceneSettings = new LuceneSearchProviderSettings(dataDirectoryPath, searchConnection.Scope);

                serviceCollection.AddSingleton(luceneSettings);
                serviceCollection.AddSingleton<ISearchProvider, LuceneSearchProvider>();
            }
        }

        public void PostInitialize(IApplicationBuilder serviceProvider)
        {
        }

        public void Uninstall()
        {
        }
    }
}
