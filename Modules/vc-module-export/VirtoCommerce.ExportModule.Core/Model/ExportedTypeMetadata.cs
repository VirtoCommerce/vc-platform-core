using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Core.Model
{
    /// <summary>
    /// Metadata for exported type: properties information and version
    /// </summary>
    public class ExportedTypeMetadata : ValueObject
    {
        public string Version { get; set; }
        /// <summary>
        /// Exportable property infos array
        /// </summary>
        public ExportedTypePropertyInfo[] PropertyInfos { get; set; }
    }
}
