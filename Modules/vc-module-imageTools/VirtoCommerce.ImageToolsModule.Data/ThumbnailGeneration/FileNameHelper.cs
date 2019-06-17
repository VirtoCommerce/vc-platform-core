using System;
using System.IO;
using System.Linq;

namespace VirtoCommerce.ImageToolsModule.Data.ThumbnailGeneration
{
    public static class FileNameHelper
    {
        public static string GenerateThumbnailName(this string fileName, string aliasName)
        {
            var name = Path.GetFileNameWithoutExtension(fileName);
            var extention = Path.GetExtension(fileName);
            var newName = string.Concat(name, "_" + aliasName, extention);

            var uri = new Uri(fileName);
            var uriWithoutLastSegment = uri.AbsoluteUri.Remove(uri.AbsoluteUri.Length - uri.Segments.Last().Length);

            var result = new Uri(new Uri(uriWithoutLastSegment), newName);

            return result.AbsoluteUri;
        }
    }
}
