using System;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ContentModule.Core.Services;

namespace VirtoCommerce.ContentModule.Azure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAzureContentBlobProvider(this IServiceCollection services, Action<AzureContentBlobOptions> setupAction = null)
        {
            services.AddSingleton<IBlobContentStorageProviderFactory, AzureContentBlobStorageProviderFactory>();
            services.AddSingleton<IBlobContentStorageProvider, AzureContentBlobStorageProvider>();
            if (setupAction != null)
            {
                services.Configure(setupAction);
            }
        }
    }
}
