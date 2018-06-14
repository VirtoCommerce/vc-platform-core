using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.ImageToolsModule.Core.Models;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ImageToolsModule.Core.ThumbnailGeneration
{
    public interface IImagesChangesProvider
    {
        bool IsTotalCountSupported { get; }

        long GetTotalChangesCount(ThumbnailTask task, DateTime? changedSince, ICancellationToken token);

        ImageChange[] GetNextChangesBatch(ThumbnailTask task, DateTime? changedSince, long? skip, long? take, ICancellationToken token);
    }
}
