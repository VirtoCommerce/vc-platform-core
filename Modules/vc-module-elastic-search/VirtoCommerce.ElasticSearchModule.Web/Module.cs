using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ElasticSearchModule.Data;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
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

                var luceneSettings = new ElasticSearchProviderSettings();

                serviceCollection.AddSingleton(luceneSettings);
                serviceCollection.AddSingleton<ISearchProvider, ElasticSearchProvider>();
            }


            //var searchConnection = snapshot.GetService<ISearchConnection>();

            //if (searchConnection?.Provider?.EqualsInvariant("ElasticSearch") == true)
            //{
            //    serviceCollection.RegisterType<ISearchProvider>(
            //        new ContainerControlledLifetimeManager(),
            //        new InjectionFactory(c => new ElasticSearchProvider(
            //            c.Resolve<ISearchConnection>(),
            //            c.Resolve<ISettingsManager>()))
            //    );
            //}

        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            // not needed
        }

        public void Uninstall()
        {
            // not needed
        }
    }
}
