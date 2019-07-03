using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.ExportModule.Data.Model
{
    public class EmptyProviderConfiguration : IExportProviderConfiguration
    {
        public string ExportedFileExtension => string.Empty;
    }
}
