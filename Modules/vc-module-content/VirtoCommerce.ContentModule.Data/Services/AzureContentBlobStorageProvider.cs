using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Assets.AzureBlobStorage;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Services
{
    public class AzureContentBlobStorageProvider : AzureBlobProvider, IContentBlobStorageProvider
    {
        private readonly string _chrootPath;
        public AzureContentBlobStorageProvider(string connectionString, string chrootPath)
            : base(connectionString)
        {
            if (chrootPath == null)
                throw new ArgumentNullException(nameof(chrootPath));

            chrootPath = chrootPath.Replace('/', '\\');
            _chrootPath = "\\" + chrootPath.TrimStart('\\');
        }

        #region IContentStorageProvider Members

        public void MoveContent(string oldUrl, string newUrl)
        {
            base.Move(oldUrl, newUrl);
        }

        public void CopyContent(string fromUrl, string toUrl)
        {
            base.Copy(fromUrl, toUrl);
        }
        #endregion
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
        public override async Task<GenericSearchResult<BlobEntry>> SearchAsync(string folderUrl, string keyword)
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
            var retVal = _chrootPath;
            if (!string.IsNullOrEmpty(url))
            {
                if (url.IsAbsoluteUrl())
                {
                    url = Uri.UnescapeDataString(new Uri(url).AbsolutePath);
                }
                retVal = "\\" + url.Replace('/', '\\').TrimStart('\\');
                retVal = _chrootPath + "\\" + retVal.Replace(_chrootPath, string.Empty);
                retVal = retVal.Replace("\\\\", "\\");
            }
            return retVal;
        }
    }
}
