using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ImageToolsModule.Core.Models;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ImageToolsModule.Core.ThumbnailGeneration
{
    public interface IThumbnailGenerationProcessor
    {
        Task ProcessTasksAsync(ThumbnailTask[] tasks, bool regenerate, Action<ThumbnailTaskProgress> progressCallback, ICancellationToken token);
    }
}
