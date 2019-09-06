using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SitemapsModule.Core.Models;

namespace VirtoCommerce.SitemapsModule.Data.Models
{
    public class SitemapItemEntity : AuditableEntity
    {
        [Required]
        [StringLength(256)]
        public string Title { get; set; }

        [StringLength(512)]
        public string ImageUrl { get; set; }

        [StringLength(128)]
        public string ObjectId { get; set; }

        [Required]
        [StringLength(128)]
        public string ObjectType { get; set; }

        [StringLength(256)]
        public string UrlTemplate { get; set; }

        #region Navigation Properties

        public string SitemapId { get; set; }
        public virtual SitemapEntity Sitemap { get; set; }

        #endregion

        public virtual SitemapItem ToModel(SitemapItem sitemapItem)
        {
            if (sitemapItem == null)
            {
                throw new ArgumentNullException(nameof(sitemapItem));
            }

            sitemapItem.Id = Id;
            sitemapItem.CreatedBy = CreatedBy;
            sitemapItem.CreatedDate = CreatedDate;
            sitemapItem.ModifiedBy = ModifiedBy;
            sitemapItem.ModifiedDate = ModifiedDate;

            sitemapItem.ImageUrl = ImageUrl;
            sitemapItem.ObjectId = ObjectId;
            sitemapItem.ObjectType = ObjectType;
            sitemapItem.SitemapId = SitemapId;
            sitemapItem.Title = Title;
            sitemapItem.UrlTemplate = UrlTemplate;

            return sitemapItem;
        }

        public virtual SitemapItemEntity FromModel(SitemapItem sitemapItem, PrimaryKeyResolvingMap pkMap)
        {
            if (sitemapItem == null)
            {
                throw new ArgumentNullException(nameof(sitemapItem));
            }
            if (pkMap == null)
            {
                throw new ArgumentNullException(nameof(pkMap));
            }

            pkMap.AddPair(sitemapItem, this);

            Id = sitemapItem.Id;
            CreatedBy = sitemapItem.CreatedBy;
            CreatedDate = sitemapItem.CreatedDate;
            ModifiedBy = sitemapItem.ModifiedBy;
            ModifiedDate = sitemapItem.ModifiedDate;

            ImageUrl = sitemapItem.ImageUrl;
            ObjectId = sitemapItem.ObjectId;
            ObjectType = sitemapItem.ObjectType;
            SitemapId = sitemapItem.SitemapId;
            Title = sitemapItem.Title;
            UrlTemplate = sitemapItem.UrlTemplate;

            return this;
        }

        public virtual void Patch(SitemapItemEntity sitemapItemEntity)
        {
            if (sitemapItemEntity == null)
            {
                throw new ArgumentNullException(nameof(sitemapItemEntity));
            }

            sitemapItemEntity.ImageUrl = ImageUrl;
            sitemapItemEntity.ObjectId = ObjectId;
            sitemapItemEntity.ObjectType = ObjectType;
            sitemapItemEntity.SitemapId = SitemapId;
            sitemapItemEntity.Title = Title;
            sitemapItemEntity.UrlTemplate = UrlTemplate;
        }
    }
}
