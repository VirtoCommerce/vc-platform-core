using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using VirtoCommerce.SitemapsModule.Core.Models;

namespace VirtoCommerce.SitemapsModule.Data.Models.Xml
{
    [Serializable]
    public class SitemapItemXmlRecord
    {
        [XmlElement("loc")]
        public string Url { get; set; }

        [XmlElement("lastmod")]
        public string ModifiedDate { get; set; }

        [XmlElement("changefreq")]
        public string UpdateFrequency { get; set; }

        [XmlElement("priority")]
        public decimal Priority { get; set; }

        [XmlElement("link", Namespace = "http://www.w3.org/1999/xhtml")]
        public List<SitemapItemAlternateLinkXmlRecord> Alternates { get; set; }

        public virtual SitemapItemXmlRecord ToXmlModel(SitemapItemRecord coreModel)
        {
            if (coreModel == null)
            {
                throw new ArgumentNullException(nameof(coreModel));
            }

            //ModifiedDate = coreModel.ModifiedDate.ToString("yyyy-MM-dd");
            Priority = coreModel.Priority;
            UpdateFrequency = coreModel.UpdateFrequency;
            Url = coreModel.Url;
            Alternates = coreModel.Alternates.Count > 0 ? coreModel.Alternates.Select(a => (new SitemapItemAlternateLinkXmlRecord()).ToXmlModel(a)).ToList() : null;

            return this;
        }
    }
}
