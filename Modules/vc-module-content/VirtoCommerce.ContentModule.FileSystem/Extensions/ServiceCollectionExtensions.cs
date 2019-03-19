using System;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ContentModule.Core.Services;

namespace VirtoCommerce.ContentModule.FileSystem.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddFileSystemContentBlobProvider(this IServiceCollection services, Action<FileSystemContentBlobOptions> setupAction = null)
        {
            services.AddSingleton<IBlobContentStorageProvider, FileSystemContentBlobStorageProvider>();
            services.AddSingleton<IBlobContentStorageProviderFactory, FileSystemContentBlobStorageProviderFactory>();
            if (setupAction != null)
            {
                services.Configure(setupAction);
            }
        }
    }
}
