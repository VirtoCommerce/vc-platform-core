using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.ExportModule.Core.Services
{
    public interface IExportProviderFactory
    {
        IExportProvider CreateProvider(string name, IExportProviderConfiguration config, ExportedTypeColumnInfo[] includedColumns);
    }
}
