using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ImageToolsModule.Data.Models
{
    public class ThumbnailTaskOptionEntity : Entity
    {
        public string ThumbnailTaskId { get; set; }

        public ThumbnailTaskEntity ThumbnailTask { get; set; }

        public string ThumbnailOptionId { get; set; }

        public ThumbnailOptionEntity ThumbnailOption { get; set; }

        public virtual void Patch(ThumbnailTaskOptionEntity target)
        {
            target.ThumbnailOptionId = ThumbnailOptionId;
            target.ThumbnailTaskId = ThumbnailTaskId;
        }
    }
}
