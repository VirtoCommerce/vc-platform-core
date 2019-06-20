using System;
using System.IO;
using System.Linq;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;

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

        public void Export(Stream stream, ExportDataRequest request, Action<ExportImportProgressInfo> progressCallback, ICancellationToken token)
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
            var exportProgress = new ExportImportProgressInfo()
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

                using (var exportProvider = _exportProviderFactory.CreateProvider(request.ProviderName, request.ProviderConfig, stream))
                {
                    //-------------------------------- Some kind of fake below. We need to decide how to limit properties & pass metadata into provider properly
                    exportedTypeDefinition.MetaData.PropertiesInfo = exportedTypeDefinition.MetaData.PropertiesInfo.Where(x => request.DataQuery.IncludedProperties.Contains(x.Name)).ToArray();
                    exportProvider.Metadata = exportedTypeDefinition.MetaData;
                    //---------------------------------

                    exportProvider.WriteMetadata(exportProvider.Metadata);

                    while (exportedCount < totalCount)
                    {
                        token.ThrowIfCancellationRequested();

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
