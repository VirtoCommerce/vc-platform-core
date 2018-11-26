using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.SitemapsModule.Core.Models
{
    public class SitemapItem : AuditableEntity
    {
        public string SitemapId { get; set; }

        public string Title { get; set; }

        public string ImageUrl { get; set; }

        public string ObjectId { get; set; }

        public string ObjectType { get; set; }

        public string UrlTemplate { get; set; }

        public ICollection<SitemapItemRecord> ItemsRecords { get; set; } = new List<SitemapItemRecord>();
    }
}