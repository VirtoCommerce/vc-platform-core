using System;
using System.Threading;
using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.ExportModule.Core.Services
{
    public interface IDataExporter
    {
        void Export(ExportDataRequest request, Action<ExportProgressInfo> progressCallback, CancellationToken token);
    }
}
