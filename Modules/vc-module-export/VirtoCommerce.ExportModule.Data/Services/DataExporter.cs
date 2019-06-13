using System;
using System.IO;
using System.Threading;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;

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

        public void Export(Stream stream, ExportDataRequest request, Action<ExportProgressInfo> progressCallback, CancellationToken token)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

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

                var exportProvider = _exportProviderFactory.CreateProvider(request.ProviderName, request.ProviderConfig, stream);

                while (exportedCount < totalCount)
                {
                    if (token.IsCancellationRequested)
                    {
                        completedMessage = "Export was cancelled by the user";
                        break;
                    }

                    exportProgress.Description = "Fetcing …";
                    progressCallback(exportProgress);

                    var objectBatch = pagedDataSource.FetchNextPage();

                    if (objectBatch == null)
                    {
                        break;
                    }

                    foreach (object obj in objectBatch)
                    {
                        try
                        {
                            exportProvider.WriteRecord(obj);
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
