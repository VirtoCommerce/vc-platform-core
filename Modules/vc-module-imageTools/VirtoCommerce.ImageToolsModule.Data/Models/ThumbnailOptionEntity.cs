using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.ImageToolsModule.Core.Models;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ImageToolsModule.Data.Models
{
    public class ThumbnailOptionEntity : AuditableEntity
    {
        [Required]
        [StringLength(1024)]
        public string Name { get; set; }

        [Required]
        [StringLength(128)]
        public string FileSuffix { get; set; }

        [Required]
        [StringLength(64)]
        public string ResizeMethod { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        public string BackgroundColor { get; set; }

        [StringLength(64)]
        public string AnchorPosition { get; set; }

        [StringLength(64)]
        public string JpegQuality { get; set; }

        public virtual ThumbnailOptionEntity FromModel(ThumbnailOption option, PrimaryKeyResolvingMap pkMap)
        {
            if (option == null) throw new ArgumentNullException(nameof(option));

            pkMap.AddPair(option, this);

            Id = option.Id;
            Name = option.Name;
            FileSuffix = option.FileSuffix;
            ResizeMethod = option.ResizeMethod.ToString();
            Width = option.Width;
            Height = option.Height;
            BackgroundColor = option.BackgroundColor;
            AnchorPosition = option.AnchorPosition.ToString();
            JpegQuality = option.JpegQuality.ToString();
            CreatedBy = option.CreatedBy;
            CreatedDate = option.CreatedDate;
            ModifiedBy = option.ModifiedBy;
            ModifiedDate = option.ModifiedDate;

            return this;
        }

        public virtual ThumbnailOption ToModel(ThumbnailOption option)
        {
            if (option == null) throw new ArgumentNullException(nameof(option));

            option.Id = Id;
            option.Name = Name;
            option.FileSuffix = FileSuffix;
            option.ResizeMethod = EnumUtility.SafeParse(ResizeMethod, Core.Models.ResizeMethod.Crop);
            option.AnchorPosition = EnumUtility.SafeParse(AnchorPosition, Core.Models.AnchorPosition.Center);
            option.JpegQuality = EnumUtility.SafeParse(JpegQuality, Core.Models.JpegQuality.High);
            option.Width = Width;
            option.Height = Height;
            option.BackgroundColor = BackgroundColor;
            option.CreatedBy = CreatedBy;
            option.CreatedDate = CreatedDate;
            option.ModifiedBy = ModifiedBy;
            option.ModifiedDate = ModifiedDate;

            return option;
        }

        public virtual void Patch(ThumbnailOptionEntity target)
        {
            target.Name = Name;
            target.FileSuffix = FileSuffix;
            target.ResizeMethod = ResizeMethod;
            target.Width = Width;
            target.Height = Height;
            target.BackgroundColor = BackgroundColor;
            target.AnchorPosition = AnchorPosition;
            target.JpegQuality = JpegQuality;
        }
    }
}
