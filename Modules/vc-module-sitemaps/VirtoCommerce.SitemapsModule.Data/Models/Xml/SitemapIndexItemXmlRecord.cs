using System;
using System.Xml.Serialization;

namespace VirtoCommerce.SitemapsModule.Data.Models.Xml
{
    [Serializable]
    public class SitemapIndexItemXmlRecord
    {
        [XmlElement("loc")]
        public string Url { get; set; }

        [XmlElement("lastmod")]
        public string ModifiedDate { get; set; }
    }
}