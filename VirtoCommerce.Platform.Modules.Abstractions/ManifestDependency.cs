using System.Xml.Serialization;

namespace VirtoCommerce.Platform.Modules.Abstractions
{
    public class ManifestDependency
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("version")]
        public string Version { get; set; }
    }
}
