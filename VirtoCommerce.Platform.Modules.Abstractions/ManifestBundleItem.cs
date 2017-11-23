using System.Xml.Serialization;

namespace VirtoCommerce.Platform.Modules.Abstractions
{
	public class ManifestBundleItem
	{
		[XmlAttribute("virtualPath")]
		public string VirtualPath { get; set; }
	}
}
