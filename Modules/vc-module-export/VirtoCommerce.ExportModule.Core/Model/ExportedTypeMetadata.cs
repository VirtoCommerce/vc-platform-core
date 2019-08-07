using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Core.Model
{
    public class ExportedTypeMetadata : ValueObject
    {
        public string Version { get; set; }
        public ExportedTypeColumnInfo[] PropertyInfos { get; set; }
    }
}
