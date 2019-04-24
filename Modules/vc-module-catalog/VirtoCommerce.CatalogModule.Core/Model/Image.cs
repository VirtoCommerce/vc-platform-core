using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public class Image : AssetBase
    {
        public int SortOrder { get; set; }
        public byte[] BinaryData { get; set; }

        public override void TryInheritFrom(IEntity parent)
        {
            if (parent is Image parentImage)
            {
                SortOrder = parentImage.SortOrder;
            }
            base.TryInheritFrom(parent);
        }
    }
}
