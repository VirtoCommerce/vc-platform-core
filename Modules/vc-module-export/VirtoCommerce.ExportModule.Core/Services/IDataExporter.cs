using System;
using System.IO;
using System.Threading;
using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.ExportModule.Core.Services
{
    public interface IDataExporter
    {
        void Export(Stream stream, ExportDataRequest request, Action<ExportProgressInfo> progressCallback, CancellationToken token);
    }
}
