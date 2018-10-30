using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ImageToolsModule.Core;
using VirtoCommerce.ImageToolsModule.Core.Models;
using VirtoCommerce.ImageToolsModule.Core.ThumbnailGeneration;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.ImageToolsModule.Data.ThumbnailGeneration
{
    public class ThumbnailGenerationProcessor : IThumbnailGenerationProcessor
    {
        private readonly IThumbnailGenerator _generator;
        private readonly ISettingsManager _settingsManager;
        private readonly IImagesChangesProvider _imageChangesProvider;

        public ThumbnailGenerationProcessor(IThumbnailGenerator generator,
            ISettingsManager settingsManager,
            IImagesChangesProvider imageChangesProvider)
        {
            _generator = generator;
            _settingsManager = settingsManager;
            _imageChangesProvider = imageChangesProvider;
        }

        public async Task ProcessTasksAsync(ICollection<ThumbnailTask> tasks, bool regenerate, Action<ThumbnailTaskProgress> progressCallback, ICancellationToken token)
        {
            var progressInfo = new ThumbnailTaskProgress { Message = "Reading the tasks..." };

            if (_imageChangesProvider.IsTotalCountSupported)
            {
                foreach (var task in tasks)
                {
                    var changedSince = regenerate ? null : task.LastRun;
                    progressInfo.TotalCount = await _imageChangesProvider.GetTotalChangesCount(task, changedSince, token);
                }
            }

            progressCallback(progressInfo);

            var pageSize = _settingsManager.GetValue(ModuleConstants.Settings.General.ProcessBatchSize.Name, 50);
            foreach (var task in tasks)
            {
                progressInfo.Message = $"Procesing task {task.Name}...";
                progressCallback(progressInfo);

                var skip = 0;
                while (true)
                {
                    var changes = await _imageChangesProvider.GetNextChangesBatch(task, regenerate ? null : task.LastRun, skip, pageSize, token);
                    if (!changes.Any())
                        break;

                    foreach (var fileChange in changes)
                    {
                        var result = await _generator.GenerateThumbnailsAsync(fileChange.Url, task.WorkPath, task.ThumbnailOptions, token);
                        progressInfo.ProcessedCount++;

                        if (!result.Errors.IsNullOrEmpty())
                        {
                            progressInfo.Errors.AddRange(result.Errors);
                        }
                    }

                    skip += changes.Length;

                    progressCallback(progressInfo);
                    token?.ThrowIfCancellationRequested();
                }
            }

            progressInfo.Message = "Finished generating thumbnails!";
            progressCallback(progressInfo);
        }
    }
}
