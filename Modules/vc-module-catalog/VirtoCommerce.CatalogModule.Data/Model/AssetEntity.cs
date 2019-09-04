using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.Model
{
    public class AssetEntity : AuditableEntity, IHasOuterId
    {
        [StringLength(2083)]
        [Required]
        public string Url { get; set; }

        [StringLength(1024)]
        public string Name { get; set; }

        [StringLength(128)]
        public string MimeType { get; set; }

        public long Size { get; set; }

        [StringLength(5)]
        public string LanguageCode { get; set; }

        [StringLength(128)]
        public string OuterId { get; set; }

        #region Navigation Properties

        public string ItemId { get; set; }
        public virtual ItemEntity CatalogItem { get; set; }

        #endregion

        public virtual Asset ToModel(Asset asset)
        {
            if (asset == null)
                throw new ArgumentNullException(nameof(asset));

            asset.Id = Id;
            asset.CreatedBy = CreatedBy;
            asset.CreatedDate = CreatedDate;
            asset.ModifiedBy = ModifiedBy;
            asset.ModifiedDate = ModifiedDate;
            asset.OuterId = OuterId;

            asset.LanguageCode = LanguageCode;
            asset.Name = Name;
            asset.MimeType = MimeType;
            asset.Url = Url;
            asset.Size = Size;

            return asset;
        }

        public virtual AssetEntity FromModel(Asset asset, PrimaryKeyResolvingMap pkMap)
        {
            if (asset == null)
                throw new ArgumentNullException(nameof(asset));

            pkMap.AddPair(asset, this);

            Id = asset.Id;
            CreatedBy = asset.CreatedBy;
            CreatedDate = asset.CreatedDate;
            ModifiedBy = asset.ModifiedBy;
            ModifiedDate = asset.ModifiedDate;
            OuterId = asset.OuterId;

            LanguageCode = asset.LanguageCode;
            Name = asset.Name;
            MimeType = asset.MimeType;
            Url = asset.Url;
            Size = asset.Size;

            return this;
        }

        public virtual void Patch(AssetEntity target)
        {
            target.LanguageCode = LanguageCode;
            target.Name = Name;
            target.MimeType = MimeType;
            target.Url = Url;
            target.Size = Size;
        }
    }
}
