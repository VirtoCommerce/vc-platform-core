using System.Xml.Serialization;

namespace VirtoCommerce.Platform.Modules.Abstractions
{
    public class ManifestBundleDirectory : ManifestBundleItem
    {
        [XmlAttribute("searchPattern")]
        public string SearchPattern { get; set; }

        [XmlAttribute("searchSubdirectories")]
        public bool SearchSubdirectories { get; set; }
    }
}
