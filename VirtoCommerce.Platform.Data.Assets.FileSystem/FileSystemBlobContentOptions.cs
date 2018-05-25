using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.Platform.Data.Assets.FileSystem
{
    public class FileSystemBlobContentOptions
    {
        public string StoragePath { get; set; }
        public string BasePublicUrl { get; set; } = "";
    }
}
