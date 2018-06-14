using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.ImageToolsModule.Core.ThumbnailGeneration
{
    public class ThumbnailGenerationResult
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
