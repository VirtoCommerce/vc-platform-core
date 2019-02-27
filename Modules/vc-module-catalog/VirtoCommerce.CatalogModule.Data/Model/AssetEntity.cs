using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.Model
{
    public class AssetEntity : AuditableEntity
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


        #region Navigation Properties

        public string ItemId { get; set; }
        public virtual ItemEntity CatalogItem { get; set; }

        #endregion


        public virtual Asset ToModel(Asset asset)
        {
            if (asset == null)
                throw new ArgumentNullException(nameof(asset));

            asset.Id = this.Id;
            asset.CreatedBy = this.CreatedBy;
            asset.CreatedDate = this.CreatedDate;
            asset.ModifiedBy = this.ModifiedBy;
            asset.ModifiedDate = this.ModifiedDate;
            asset.LanguageCode = this.LanguageCode;
            asset.Name = this.Name;
            asset.MimeType = this.MimeType;
            asset.Url = this.Url;
            asset.Size = this.Size;

            return asset;

        }

        public virtual AssetEntity FromModel(Asset asset, PrimaryKeyResolvingMap pkMap)
        {
            if (asset == null)
                throw new ArgumentNullException(nameof(asset));

            pkMap.AddPair(asset, this);

            this.Id = asset.Id;
            this.CreatedBy = asset.CreatedBy;
            this.CreatedDate = asset.CreatedDate;
            this.ModifiedBy = asset.ModifiedBy;
            this.ModifiedDate = asset.ModifiedDate;
            this.LanguageCode = asset.LanguageCode;
            this.Name = asset.Name;
            this.MimeType = asset.MimeType;
            this.Url = asset.Url;
            this.Size = asset.Size;

            return this;
        }

        public virtual void Patch(AssetEntity target)
        {
            target.LanguageCode = this.LanguageCode;
            target.Name = this.Name;
            target.MimeType = this.MimeType;
            target.Url = this.Url;
            target.Size = this.Size;            
        }
    }
}
