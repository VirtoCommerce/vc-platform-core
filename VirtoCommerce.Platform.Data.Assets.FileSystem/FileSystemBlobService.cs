using System;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core.Assets;

namespace VirtoCommerce.Platform.Data.Assets.FileSystem
{
    public static class FileSystemBlobService
    {
        public static void AddFileSystemBlobProvider(this IServiceCollection services, Action<FileSystemBlobContentOptions> setupAction = null)
        {
            services.AddSingleton<IBlobStorageProvider, FileSystemBlobProvider>();
            services.AddSingleton<IBlobUrlResolver, FileSystemBlobProvider>();
            if (setupAction != null)
            {
                services.Configure(setupAction);
            }
        }
    }
}
