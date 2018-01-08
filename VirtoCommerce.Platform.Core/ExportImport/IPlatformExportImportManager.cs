using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace VirtoCommerce.Platform.Core.ExportImport
{
    public interface IPlatformExportImportManager
    {
        PlatformExportManifest GetNewExportManifest(string author);
        PlatformExportManifest ReadExportManifest(Stream stream);
        Task ExportAsync(Stream outStream, PlatformExportManifest exportOptions, Action<ExportImportProgressInfo> progressCallback, CancellationToken cancellationToken);
        Task ImportAsync(Stream inputStream, PlatformExportManifest importOptions, Action<ExportImportProgressInfo> progressCallback, CancellationToken cancellationToken);
    }
}
