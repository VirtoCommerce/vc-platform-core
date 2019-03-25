using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.LuceneSearchModule.Data;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
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
            var searchProvider = snapshot.GetService<IOptions<SearchSettings>>();
            var configuration = snapshot.GetService<IConfiguration>();

            if (searchProvider.Value != null && searchProvider.Value.Provider.EqualsInvariant("Lucene"))
            {
                serviceCollection.Configure<SearchSettings>(o =>
                {
                    o.ConnectionSettings = configuration.GetSection($"Search:{searchProvider.Value.Provider}:SearchConnectionString").Get<LuceneSearchConnectionSettings>();
                });
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
