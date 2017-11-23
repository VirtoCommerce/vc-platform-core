using System.Xml.Serialization;

namespace VirtoCommerce.Platform.Modules.Abstractions
{
    public class ModulePermissionGroup
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("permission")]
        public ModulePermission[] Permissions { get; set; }
    }
}
