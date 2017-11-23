using System.Xml.Serialization;

namespace VirtoCommerce.Platform.Modules.Abstractions
{
    public class ModuleSettingsGroup
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("setting")]
        public ModuleSetting[] Settings { get; set; }
    }
}
