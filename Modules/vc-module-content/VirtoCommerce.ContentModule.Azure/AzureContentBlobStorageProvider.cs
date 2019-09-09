using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Assets.AzureBlobStorage;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Azure
{
    public class AzureContentBlobStorageProvider : AzureBlobProvider, IBlobContentStorageProvider
    {
        private AzureContentBlobOptions _options;
        public AzureContentBlobStorageProvider(IOptions<AzureContentBlobOptions> options)
            : base(options)
        {
            _options = options.Value;
        }

        public override Stream OpenRead(string url)
        {
            return base.OpenRead(NormalizeUrl(url));
        }

        public override async Task CreateFolderAsync(BlobFolder folder)
        {
            if (folder == null)
                throw new ArgumentNullException(nameof(folder));

            if (folder.ParentUrl.IsNullOrEmpty())
            {
                folder.Name = NormalizeUrl(folder.Name);
            }
            await base.CreateFolderAsync(folder);
        }

        public override Stream OpenWrite(string url)
        {
            return base.OpenWrite(NormalizeUrl(url));
        }

        public override async Task RemoveAsync(string[] urls)
        {
            urls = urls.Select(NormalizeUrl).ToArray();

            await base.RemoveAsync(urls);
        }

        public override async Task<BlobEntrySearchResult> SearchAsync(string folderUrl, string keyword)
        {
            folderUrl = NormalizeUrl(folderUrl);
            return await base.SearchAsync(folderUrl, keyword);
        }

        /// <summary>
        /// Chroot url (artificial add parent 'chroot' folder)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string NormalizeUrl(string url)
        {
            var retVal = _options.RootPath;
            if (!string.IsNullOrEmpty(url))
            {
                if (url.IsAbsoluteUrl())
                {
                    url = Uri.UnescapeDataString(new Uri(url).AbsolutePath);
                }
                retVal = Path.DirectorySeparatorChar + url.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);
                retVal = _options.RootPath + Path.DirectorySeparatorChar + retVal.Replace(_options.RootPath, string.Empty);
                retVal = retVal.Replace($"{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}", $"{Path.DirectorySeparatorChar}");
            }
            return retVal;
        }
    }
}
