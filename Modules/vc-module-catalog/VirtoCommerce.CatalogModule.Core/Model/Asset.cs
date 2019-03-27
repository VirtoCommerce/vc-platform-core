using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public class Asset : AssetBase
    {
        public string MimeType { get; set; }
        public long Size { get; set; }
        public string ReadableSize
        {
            get
            {
                return Size.ToHumanReadableSize();
            }
        }

        public byte[] BinaryData { get; set; }

        public override void TryInheritFrom(IEntity parent)
        {
            if (parent is Asset parentAsset)
            {
                MimeType = parentAsset.MimeType;
                Size = parentAsset.Size;
            }
            base.TryInheritFrom(parent);
        }
    }
}
