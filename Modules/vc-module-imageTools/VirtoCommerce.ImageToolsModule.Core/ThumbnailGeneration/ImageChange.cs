using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ImageToolsModule.Core.ThumbnailGeneration
{
    public class ImageChange : ValueObject
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public EntryState ChangeState { get; set; }
    }
}
