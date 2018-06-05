using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core.Assets;

namespace VirtoCommerce.Platform.Data.Assets.AzureBlobStorage
{
    public static class AzureBlobService
    {
        public static void AddAzureBlobProvider(this IServiceCollection services, Action<AzureBlobContentOptions> setupAction = null)
        {
            services.AddSingleton<IBlobStorageProvider, AzureBlobProvider>();
            services.AddSingleton<IBlobUrlResolver, AzureBlobProvider>();
            if (setupAction != null)
            {
                services.Configure(setupAction);
            }
        }
    }
}
