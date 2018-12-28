using System;
using System.Collections.Generic;

namespace VirtoCommerce.SitemapsModule.Core.Models
{
    public class SitemapItemRecord
    {
        public string Url { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string UpdateFrequency { get; set; }

        public decimal Priority { get; set; }

        public ICollection<SitemapItemAlternateLinkRecord> Alternates { get; set; } = new List<SitemapItemAlternateLinkRecord>();
    }
}