using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SitemapsModule.Core.Models;

namespace VirtoCommerce.SitemapsModule.Data.Models
{
    public class SitemapEntity : AuditableEntity
    {
        [Required]
        [StringLength(256)]
        public string Filename { get; set; }

        [Required]
        [StringLength(64)]
        public string StoreId { get; set; }

        [StringLength(256)]
        public string UrlTemplate { get; set; }

        [NotMapped]
        public int TotalItemsCount { get; set; }

        #region Navigation Properties

        public virtual ObservableCollection<SitemapItemEntity> Items { get; set; }
            = new NullCollection<SitemapItemEntity>();

        #endregion

        public virtual Sitemap ToModel(Sitemap sitemap)
        {
            if (sitemap == null)
            {
                throw new ArgumentNullException(nameof(sitemap));
            }

            sitemap.Id = Id;
            sitemap.CreatedBy = CreatedBy;
            sitemap.CreatedDate = CreatedDate;
            sitemap.ModifiedBy = ModifiedBy;
            sitemap.ModifiedDate = ModifiedDate;

            sitemap.Location = Filename;
            sitemap.StoreId = StoreId;
            sitemap.UrlTemplate = UrlTemplate;
            sitemap.TotalItemsCount = TotalItemsCount;

            return sitemap;
        }

        public virtual SitemapEntity FromModel(Sitemap sitemap, PrimaryKeyResolvingMap pkMap)
        {
            if (sitemap == null)
            {
                throw new ArgumentNullException(nameof(sitemap));
            }
            if (pkMap == null)
            {
                throw new ArgumentNullException(nameof(pkMap));
            }

            pkMap.AddPair(sitemap, this);

            Id = sitemap.Id;
            CreatedBy = sitemap.CreatedBy;
            CreatedDate = sitemap.CreatedDate;
            ModifiedBy = sitemap.ModifiedBy;
            ModifiedDate = sitemap.ModifiedDate;

            Filename = sitemap.Location;
            StoreId = sitemap.StoreId;
            UrlTemplate = sitemap.UrlTemplate;

            return this;
        }

        public virtual void Patch(SitemapEntity sitemapEntity)
        {
            if (sitemapEntity == null)
            {
                throw new ArgumentNullException(nameof(sitemapEntity));
            }

            sitemapEntity.Filename = Filename;
            sitemapEntity.StoreId = StoreId;
            sitemapEntity.UrlTemplate = UrlTemplate;
        }
    }
}
