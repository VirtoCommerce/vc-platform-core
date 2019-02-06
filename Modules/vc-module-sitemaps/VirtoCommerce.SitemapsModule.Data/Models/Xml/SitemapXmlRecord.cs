using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace VirtoCommerce.SitemapsModule.Data.Models.Xml
{
    [Serializable]
    [XmlRoot("urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
    public class SitemapXmlRecord
    {
        public SitemapXmlRecord()
        {
            Items = new List<SitemapItemXmlRecord>();
        }

        [XmlElement("url")]
        public List<SitemapItemXmlRecord> Items { get; set; }
    }
}