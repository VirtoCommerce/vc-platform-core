using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ImageToolsModule.Core.Models
{
    public class ThumbnailOption : AuditableEntity, ICloneable
    {
        public string Name { get; set; }

        public string FileSuffix { get; set; }

        public ResizeMethod ResizeMethod { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        public string BackgroundColor { get; set; }

        public AnchorPosition AnchorPosition { get; set; }

        public JpegQuality JpegQuality { get; set; }

        #region ICloneable members

        public virtual object Clone()
        {
            return MemberwiseClone() as ThumbnailOption;
        }

        #endregion
    }
}
