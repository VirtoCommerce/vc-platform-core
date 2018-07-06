using System;
using System.Collections.Generic;
using System.Text;
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
