using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using VirtoCommerce.SitemapsModule.Core.Models;

namespace VirtoCommerce.SitemapsModule.Data.Models.Xml
{
    [Serializable]
    public class SitemapItemAlternateLinkXmlRecord
    {
        [XmlAttribute("href")]
        public string Url { get; set; }

        [XmlAttribute("hreflang")]
        public string Language { get; set; }

        [XmlAttribute("rel")]
        public string Type { get; set; }

        public virtual SitemapItemAlternateLinkXmlRecord ToXmlModel(SitemapItemAlternateLinkRecord coreModel)
        {
            if (coreModel == null)
            {
                throw new ArgumentNullException("coreModel");
            }

            Url = coreModel.Url;
            Language = coreModel.Language;
            Type = coreModel.Type;

            return this;
        }
    }
}
