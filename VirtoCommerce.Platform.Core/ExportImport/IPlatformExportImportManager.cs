using System;
using System.IO;

namespace VirtoCommerce.Platform.Core.ExportImport
{
    public interface IPlatformExportImportManager
    {
        PlatformExportManifest GetNewExportManifest(string author);
        PlatformExportManifest ReadExportManifest(Stream stream);
        void Export(Stream outStream, PlatformExportManifest exportOptions, Action<ExportImportProgressInfo> progressCallback);
        void Import(Stream inputStream, PlatformExportManifest importOptions, Action<ExportImportProgressInfo> progressCallback);
    }
}
