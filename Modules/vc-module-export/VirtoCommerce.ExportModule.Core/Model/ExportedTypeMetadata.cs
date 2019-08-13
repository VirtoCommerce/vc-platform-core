using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Core.Model
{
    /// <summary>
    /// Metadata for single exported type: columns set and version
    /// </summary>
    public class ExportedTypeMetadata : ValueObject
    {
        public string Version { get; set; }
        /// <summary>
        /// Set of columns to export
        /// </summary>
        public ExportedTypeColumnInfo[] PropertyInfos { get; set; }
    }
}
