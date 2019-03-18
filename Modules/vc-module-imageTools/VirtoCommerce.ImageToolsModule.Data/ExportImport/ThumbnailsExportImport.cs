using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using VirtoCommerce.ImageToolsModule.Core.ExportImport;
using VirtoCommerce.ImageToolsModule.Core.Models;
using VirtoCommerce.ImageToolsModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;

namespace VirtoCommerce.ImageToolsModule.Data.ExportImport
{
    public class ThumbnailsExportImport : IExportSupport, IImportSupport
    {
        private readonly IThumbnailTaskSearchService _taskSearchService;
        private readonly IThumbnailOptionSearchService _optionSearchService;
        private readonly IThumbnailTaskService _taskService;
        private readonly IThumbnailOptionService _optionService;

        private const int _batchSize = 50;
        private readonly JsonSerializer _serializer;

        public ThumbnailsExportImport(IThumbnailTaskSearchService taskSearchService,
            IThumbnailOptionSearchService optionSearchService, IThumbnailTaskService taskService,
            IThumbnailOptionService optionService, IOptions<MvcJsonOptions> jsonOptions)
        {
            _taskSearchService = taskSearchService;
            _optionSearchService = optionSearchService;
            _taskService = taskService;
            _optionService = optionService;

            _serializer = JsonSerializer.Create(jsonOptions.Value.SerializerSettings);
        }

        public async Task ExportAsync(Stream outStream, ExportImportOptions exportOptions, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            var moduleExportOptions = exportOptions as ThumbnailExportImportOptions;
            cancellationToken.ThrowIfCancellationRequested();

            var progressInfo = new ExportImportProgressInfo { Description = "loading data..." };
            progressCallback(progressInfo);

            using (var sw = new StreamWriter(outStream, Encoding.UTF8))
            {
                using (var writer = new JsonTextWriter(sw))
                {
                    writer.WriteStartObject();

                    await ExportOptions(writer, _serializer, progressInfo, progressCallback, cancellationToken);
                    await ExportTasksAsync(writer, _serializer, progressInfo, progressCallback, cancellationToken);

                    writer.WriteEndObject();
                    writer.Flush();
                }
            }
        }

        private async Task ExportOptions(JsonTextWriter writer, JsonSerializer serializer, ExportImportProgressInfo progressInfo, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            progressInfo.Description = "Exporting options...";
            progressCallback(progressInfo);

            var thumbnailOption = await _optionSearchService.SearchAsync(new ThumbnailOptionSearchCriteria { Take = 0, Skip = 0 });
            var totalCount = thumbnailOption.TotalCount;

            writer.WritePropertyName("OptionsTotalCount");
            writer.WriteValue(totalCount);

            writer.WritePropertyName("Options");
            writer.WriteStartArray();

            for (int i = 0; i < totalCount; i += _batchSize)
            {
                var options = await _optionSearchService.SearchAsync(new ThumbnailOptionSearchCriteria { Take = _batchSize, Skip = i });

                foreach (var option in options.Results)
                {
                    serializer.Serialize(writer, option);
                }

                writer.Flush();
                progressInfo.Description = $"{Math.Min(totalCount, i + _batchSize)} of {totalCount} options exported";
                progressCallback(progressInfo);
            }

            writer.WriteEndArray();
        }

        private async Task ExportTasksAsync(JsonTextWriter writer, JsonSerializer serializer, ExportImportProgressInfo progressInfo, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            progressInfo.Description = "Exporting tasks...";
            progressCallback(progressInfo);

            var thumbnailTask = await _taskSearchService.SearchAsync(new ThumbnailTaskSearchCriteria() { Take = 0, Skip = 0 });
            var totalCount = thumbnailTask.TotalCount;

            writer.WritePropertyName("TakskTotalCount");
            writer.WriteValue(totalCount);

            writer.WritePropertyName("Tasks");
            writer.WriteStartArray();

            for (int i = 0; i < totalCount; i += _batchSize)
            {
                var tasks = await _taskSearchService.SearchAsync(new ThumbnailTaskSearchCriteria { Take = _batchSize, Skip = i });

                foreach (var task in tasks.Results)
                {
                    serializer.Serialize(writer, task);
                }

                writer.Flush();
                progressInfo.Description = $"{Math.Min(totalCount, i + _batchSize)} of {totalCount} tasks exported";
                progressCallback(progressInfo);
            }

            writer.WriteEndArray();
        }


        public async Task ImportAsync(Stream inputStream, ExportImportOptions importOptions, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            var moduleImportOptions = importOptions as ThumbnailExportImportOptions;

            cancellationToken.ThrowIfCancellationRequested();

            var progressInfo = new ExportImportProgressInfo();
            using (var streamReader = new StreamReader(inputStream))
            {
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    while (jsonReader.Read())
                    {
                        if (jsonReader.TokenType != JsonToken.PropertyName)
                            continue;

                        switch (jsonReader.Value.ToString())
                        {
                            case "Options":
                                jsonReader.Read();
                                var options = _serializer.Deserialize<ThumbnailOption[]>(jsonReader);
                                progressInfo.Description = $"Importing {options.Length} options...";
                                progressCallback(progressInfo);
                                await _optionService.SaveOrUpdateAsync(options);
                                break;
                            case "Tasks":
                                jsonReader.Read();
                                var tasks = _serializer.Deserialize<ThumbnailTask[]>(jsonReader);
                                progressInfo.Description = $"Importing {tasks.Length} tasks...";
                                progressCallback(progressInfo);
                                await _taskService.SaveChangesAsync(tasks);
                                break;
                        }
                    }
                }
            }
        }
    }
}
