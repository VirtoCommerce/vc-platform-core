using System.IO;
using Microsoft.AspNetCore.Mvc.TagHelpers.Internal;
using Microsoft.AspNetCore.WebUtilities;

namespace VirtoCommerce.Platform.Core.FileVersionProvider
{
    public class FileVersionProvider : IFileVersionProvider
    {
        public string GetFileVersion(string fullPath)
        {
            if (!File.Exists(fullPath))
            {
                return null;
            }

            using (var sha256 = CryptographyAlgorithms.CreateSHA256())
            using (var readStream = new FileInfo(fullPath).OpenRead())
            {
                //TODO: Caching
                return WebEncoders.Base64UrlEncode(sha256.ComputeHash(readStream));
            }
        }
    }
}
