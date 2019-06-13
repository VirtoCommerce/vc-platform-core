using System;
using System.Threading;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;

namespace VirtoCommerce.ExportModule.Data.Services
{
    public class DataExporter : IDataExporter
    {
        private readonly IKnownExportTypesResolver _exportTypesResolver;
        private readonly IExportProvider _exportProvider;

        public DataExporter(IKnownExportTypesResolver exportTypesResolver)
        {
            _exportTypesResolver = exportTypesResolver;
        }

        public void Export(ExportDataRequest request, Action<ExportProgressInfo> progressCallback, CancellationToken token)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var exportedTypeDefinition = _exportTypesResolver.ResolveExportedTypeDefinition(request.ExportTypeName);
            var pagedDataSource = exportedTypeDefinition.ExportedDataSourceFactory(request.DataQuery);
            var totalCount = pagedDataSource.GetTotalCount();
            var exportedCount = 0;

            while (exportedCount < totalCount)
            {
                var objectBatch = pagedDataSource.FetchNextPage();

                if (objectBatch == null)
                {
                    break;
                }

                foreach (object obj in objectBatch)
                {
                    _exportProvider.WriteRecord(obj);
                    exportedCount++;
                }
            }
        }
    }
}
