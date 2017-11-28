using System;
using System.IO;

namespace VirtoCommerce.Platform.Core.ExportImport
{
    public interface ISupportExportImportModule
	{
		string ExportDescription { get; }
		void DoExport(Stream outStream, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback);
		void DoImport(Stream inputStream, PlatformExportManifest manifest, Action<ExportImportProgressInfo> progressCallback);
	}
}
