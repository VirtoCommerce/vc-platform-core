using System;
using System.IO;
using System.Linq;
using System.Text;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Data.Services
{
    public class DataExporter : IDataExporter
    {
        private readonly IKnownExportTypesResolver _exportTypesResolver;
        private readonly IExportProviderFactory _exportProviderFactory;

        public DataExporter(IKnownExportTypesResolver exportTypesResolver, IExportProviderFactory exportProviderFactory)
        {
            _exportTypesResolver = exportTypesResolver;
            _exportProviderFactory = exportProviderFactory;
        }

        public void Export(Stream stream, ExportDataRequest request, Action<ExportProgressInfo> progressCallback, ICancellationToken token)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            token.ThrowIfCancellationRequested();

            var exportedTypeDefinition = _exportTypesResolver.ResolveExportedTypeDefinition(request.ExportTypeName);
            var pagedDataSource = exportedTypeDefinition.ExportedDataSourceFactory(request.DataQuery);

            var completedMessage = $"Export completed";
            var totalCount = pagedDataSource.GetTotalCount();
            var exportedCount = 0;
            var exportProgress = new ExportProgressInfo()
            {
                ProcessedCount = 0,
                TotalCount = totalCount,
                Description = "Export has started",
            };

            progressCallback(exportProgress);

            try
            {
                exportProgress.Description = "Creating provider…";
                progressCallback(exportProgress);

                using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true) { AutoFlush = true })
                using (var exportProvider = _exportProviderFactory.CreateProvider(request.ProviderName, request.ProviderConfig))
                {
                    var filteredMetadata = (ExportedTypeMetadata)exportedTypeDefinition.MetaData.Clone();

                    var includedColumnNames = request.DataQuery.IncludedColumns.Select(x => x.Name).ToArray();
                    filteredMetadata.PropertyInfos = exportedTypeDefinition.MetaData.PropertyInfos
                        .Where(x => request.DataQuery.IncludedColumns.IsNullOrEmpty() || includedColumnNames.Contains(x.Name))
                        .ToArray();

                    exportProvider.Metadata = filteredMetadata;

                    exportProgress.Description = "Fetching…";
                    progressCallback(exportProgress);

                    while (exportedCount < totalCount)
                    {
                        token.ThrowIfCancellationRequested();

                        var objectBatch = pagedDataSource.FetchNextPage();

                        if (objectBatch == null)
                        {
                            break;
                        }

                        foreach (object obj in objectBatch)
                        {
                            try
                            {
                                exportProvider.WriteRecord(writer, obj);
                            }
                            catch (Exception e)
                            {
                                exportProgress.Errors.Add(e.Message);
                                progressCallback(exportProgress);
                            }
                            exportedCount++;
                        }

                        exportProgress.ProcessedCount = exportedCount;

                        if (exportedCount != totalCount)
                        {
                            exportProgress.Description = $"{exportedCount} out of {totalCount} have been exported.";
                            progressCallback(exportProgress);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                exportProgress.Errors.Add(e.Message);
            }
            finally
            {
                if (exportProgress.Errors.Count > 0)
                {
                    completedMessage = $"Export completed with errors";
                }

                exportProgress.Description = $"{completedMessage}: {exportedCount} out of {totalCount} have been exported.";
                progressCallback(exportProgress);
            }
        }
    }
}
