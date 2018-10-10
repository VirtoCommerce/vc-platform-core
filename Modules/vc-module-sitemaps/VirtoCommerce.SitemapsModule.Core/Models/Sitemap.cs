using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.SitemapsModule.Core.Models
{
    public class Sitemap : AuditableEntity
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
    }
}
