using System;
using System.IO;
using System.Threading;

namespace VirtoCommerce.Platform.Core.ExportImport
{
    public interface ISupportExportImportModule
	{
        void DoExport(Stream outStream, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback, CancellationToken cancellationToken);
		void DoImport(Stream inputStream, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback, CancellationToken cancellationToken);
	}
}
