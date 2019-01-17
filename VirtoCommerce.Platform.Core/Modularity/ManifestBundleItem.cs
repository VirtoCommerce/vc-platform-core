using System.Collections.Generic;
using System.Xml.Serialization;

namespace VirtoCommerce.Platform.Core.Modularity
{
    public class ManifestBundleItem
    {
        [XmlElement(ElementName = "file")]
        public List<string> FileName { get; set; }

        [XmlAttribute(AttributeName = "virtualPath")]
        public string VirtualPath { get; set; }
    }
}
