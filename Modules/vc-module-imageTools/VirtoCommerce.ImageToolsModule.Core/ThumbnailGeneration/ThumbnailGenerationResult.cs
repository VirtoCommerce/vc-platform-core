using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ImageToolsModule.Core.ThumbnailGeneration
{
    public class ThumbnailGenerationResult : ValueObject
    {
        public IList<string> GeneratedThumbnails { get; }

        public IList<string> Errors { get; }

        public ThumbnailGenerationResult()
        {
            GeneratedThumbnails = new List<string>();
            Errors = new List<string>();
        }
    }
}
