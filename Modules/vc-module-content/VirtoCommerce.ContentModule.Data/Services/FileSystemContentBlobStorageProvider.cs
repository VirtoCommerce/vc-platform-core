using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Assets.FileSystem;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Services
{
    public class FileSystemContentBlobStorageProvider : FileSystemBlobProvider, IContentStorageProviderFactory
    {
        public FileSystemContentBlobStorageProvider(IOptions<FileSystemBlobContentOptions> options, IHttpContextAccessor httpContext)
            : base(options, httpContext)
        {
        }

        #region IContentStorageProvider Members

        public void MoveContent(string srcUrl, string dstUrl)
        {
            var srcPath = GetStoragePathFromUrl(srcUrl);
            var dstPath = GetStoragePathFromUrl(dstUrl);

            if (srcPath != dstPath)
            {
                if (Directory.Exists(srcPath) && !Directory.Exists(dstPath))
                {
                    Directory.Move(srcPath, dstPath);
                }
                else if (File.Exists(srcPath) && !File.Exists(dstPath))
                {
                    File.Move(srcPath, dstPath);
                }
            }
        }

        public void CopyContent(string srcUrl, string destUrl)
        {
            var srcPath = GetStoragePathFromUrl(srcUrl);
            var destPath = GetStoragePathFromUrl(destUrl);

            CopyDirectoryRecursive(srcPath, destPath);
        }

        #endregion

        public override async Task<GenericSearchResult<BlobEntry>> SearchAsync(string folderUrl, string keyword)
        {
            return await base.SearchAsync(folderUrl, keyword);
        }

        private static void CopyDirectoryRecursive(string sourcePath, string destPath)
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }

            foreach (var file in Directory.GetFiles(sourcePath))
            {
                var dest = Path.Combine(destPath, Path.GetFileName(file));
                File.Copy(file, dest);
            }

            foreach (var folder in Directory.GetDirectories(sourcePath))
            {
                var dest = Path.Combine(destPath, Path.GetFileName(folder));
                CopyDirectoryRecursive(folder, dest);
            }
        }
    }
}
