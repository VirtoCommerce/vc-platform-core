using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace VirtoCommerce.SitemapsModule.Data.Models.Xml
{
    [Serializable]
    [XmlRoot("sitemapindex", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
    public class SitemapIndexXmlRecord
    {
        public SitemapIndexXmlRecord()
        {
            Sitemaps = new List<SitemapIndexItemXmlRecord>();
        }

        [XmlElement("sitemap")]
        public List<SitemapIndexItemXmlRecord> Sitemaps { get; set; }
    }
}