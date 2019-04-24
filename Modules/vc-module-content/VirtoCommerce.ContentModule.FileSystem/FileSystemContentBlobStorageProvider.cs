using Microsoft.Extensions.Options;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Assets.FileSystem;

namespace VirtoCommerce.ContentModule.FileSystem
{
    public class FileSystemContentBlobStorageProvider : FileSystemBlobProvider, IBlobContentStorageProvider
    {
        public FileSystemContentBlobStorageProvider(IOptions<FileSystemContentBlobOptions> options)
            : base(options)
        {
        }

    }
}
