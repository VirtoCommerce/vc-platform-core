using System.IO;
using Microsoft.Extensions.Options;
using VirtoCommerce.ContentModule.Core.Services;

namespace VirtoCommerce.ContentModule.FileSystem
{
    public class FileSystemContentBlobStorageProviderFactory : IBlobContentStorageProviderFactory
    {
        private readonly FileSystemContentBlobOptions _options;
        public FileSystemContentBlobStorageProviderFactory(IOptions<FileSystemContentBlobOptions> options)
        {
            _options = options.Value;
        }
        public IBlobContentStorageProvider CreateProvider(string basePath)
        {
            var clonedOptions = _options.Clone() as FileSystemContentBlobOptions;

            var storagePath = Path.Combine(clonedOptions.RootPath, basePath.Replace("/", "\\"));
            //Use content api/content as public url by default             
            var publicPath = $"~/api/content/{basePath}?relativeUrl=";
            if (!string.IsNullOrEmpty(clonedOptions.PublicUrl))
            {
                publicPath = clonedOptions.PublicUrl + "/" + basePath;
            }
            clonedOptions.RootPath = storagePath;
            clonedOptions.PublicUrl = publicPath;
            //Do not export default theme (Themes/default) its will distributed with code
            return new FileSystemContentBlobStorageProvider(new OptionsWrapper<FileSystemContentBlobOptions>(clonedOptions));
        }
    }
}
