using System;
using System.IO;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;

namespace VirtoCommerce.ExportModule.Core.Services
{
    public interface IDataExporter
    {
        void Export(Stream stream, ExportDataRequest request, Action<ExportImportProgressInfo> progressCallback, ICancellationToken token);
    }
}
