using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.SitemapsModule.Core.Models
{
    public class Sitemap : AuditableEntity, ICloneable
    {
        public Sitemap()
        {
            Items = new List<SitemapItem>();
            PagedLocations = new List<string>();
        }

        public string Location { get; set; }

        public string StoreId { get; set; }

        public ICollection<SitemapItem> Items { get; set; }

        public string UrlTemplate { get; set; }

        public int TotalItemsCount { get; set; }

        public ICollection<string> PagedLocations { get; set; }



        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as Sitemap;

            result.Items = Items?.Select(x => x.Clone()).OfType<SitemapItem>().ToList();

            return result;
        }

        #endregion
    }
}
