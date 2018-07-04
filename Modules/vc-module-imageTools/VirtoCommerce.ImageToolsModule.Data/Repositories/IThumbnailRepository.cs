using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ImageToolsModule.Data.Models;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ImageToolsModule.Data.Repositories
{
    public interface IThumbnailRepository : IRepository
    {
        /// <summary>
        /// Gets the thumbnail tasks entities.
        /// </summary>
        IQueryable<ThumbnailTaskEntity> ThumbnailTasks { get; }

        /// <summary>
        /// Gets the thumbnail options entities.
        /// </summary>
        IQueryable<ThumbnailOptionEntity> ThumbnailOptions { get; }

        Task<ThumbnailTaskEntity[]> GetThumbnailTasksByIdsAsync(string[] ids);

        Task<ThumbnailOptionEntity[]> GetThumbnailOptionsByIdsAsync(string[] ids);

        Task RemoveThumbnailTasksByIdsAsync(string[] ids);

        Task RemoveThumbnailOptionsByIds(string[] ids);
    }
}
