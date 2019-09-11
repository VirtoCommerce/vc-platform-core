using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.ExportModule.Core.Services
{
    /// <summary>
    /// Interface for implementing a factory to create export providers.
    /// </summary>
    public interface IExportProviderFactory
    {
        IExportProvider CreateProvider(ExportDataRequest exportDataRequest);
    }
}
