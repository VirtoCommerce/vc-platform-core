using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VirtoCommerce.ContentModule.Core.Services;

namespace VirtoCommerce.ContentModule.FileSystem
{
    public class FileSystemContentBlobStorageProviderFactory : IBlobContentStorageProviderFactory
    {
        private readonly FileSystemContentBlobOptions _options;
        private readonly IUrlHelper _urlHelper;
        public FileSystemContentBlobStorageProviderFactory(IOptions<FileSystemContentBlobOptions> options, IUrlHelper urlHelper)
        {
            _options = options.Value;
            _urlHelper = urlHelper;
        }
        public IBlobContentStorageProvider CreateProvider(string basePath)
        {
            var clonedOptions = _options.Clone() as FileSystemContentBlobOptions;

            var storagePath = Path.Combine(clonedOptions.RootPath, basePath.Replace("/", "\\"));
            //Use content api/content as public url by default             
            var publicPath = $"~/api/content/{basePath}?relativeUrl=";
            if (!string.IsNullOrEmpty(clonedOptions.PublicPath))
            {
                publicPath = clonedOptions.PublicPath + "/" + basePath;
            }
            clonedOptions.RootPath = storagePath;
            clonedOptions.PublicPath = publicPath;
            //Do not export default theme (Themes/default) its will distributed with code
            return new FileSystemContentBlobStorageProvider(new OptionsWrapper<FileSystemContentBlobOptions>(clonedOptions), _urlHelper);
        }
    }
}
