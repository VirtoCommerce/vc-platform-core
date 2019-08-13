using System;
using System.IO;
using System.Text;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.ExportModule.Data.Extensions;
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
                using (var exportProvider = _exportProviderFactory.CreateProvider(request.ProviderName, request.ProviderConfig, request.DataQuery.IncludedProperties))
                {
                    var needTabularData = exportProvider.IsTabular;

                    if (needTabularData && !exportedTypeDefinition.IsTabularExportSupported)
                    {
                        throw new NotSupportedException($"Provider \"{exportProvider.TypeName}\" does not support tabular export.");
                    }

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

                        foreach (var obj in objectBatch)
                        {
                            try
                            {
                                var preparedObject = obj.Clone();

                                request.DataQuery.FilterProperties(preparedObject);

                                if (needTabularData)
                                {
                                    preparedObject = exportedTypeDefinition.TabularDataConverter.ToTabular(preparedObject);
                                }

                                exportProvider.WriteRecord(writer, preparedObject);
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
