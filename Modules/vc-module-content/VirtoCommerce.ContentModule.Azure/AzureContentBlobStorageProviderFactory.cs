using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Options;
using VirtoCommerce.ContentModule.Core.Services;

namespace VirtoCommerce.ContentModule.Azure
{
    public class AzureContentBlobStorageProviderFactory : IBlobContentStorageProviderFactory
    {
        private readonly AzureContentBlobOptions _options;
        public AzureContentBlobStorageProviderFactory(IOptions<AzureContentBlobOptions> options)
        {
            _options = options.Value;
        }
        public IBlobContentStorageProvider CreateProvider(string basePath)
        {
            var clonedOptions = _options.Clone() as AzureContentBlobOptions;
            clonedOptions.RootPath = Path.Combine(clonedOptions.RootPath, basePath);
            return new AzureContentBlobStorageProvider(new OptionsWrapper<AzureContentBlobOptions>(clonedOptions));
        }
    }
}
