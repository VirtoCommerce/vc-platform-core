using System;
using VirtoCommerce.Platform.Assets.FileSystem;

namespace VirtoCommerce.ContentModule.FileSystem
{
    public class FileSystemContentBlobOptions : FileSystemBlobOptions, ICloneable
    {
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
