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
        /// Information about potentially exported properties
        /// </summary>
        public ExportedTypePropertyInfo[] PropertyInfos { get; set; }
    }
}
