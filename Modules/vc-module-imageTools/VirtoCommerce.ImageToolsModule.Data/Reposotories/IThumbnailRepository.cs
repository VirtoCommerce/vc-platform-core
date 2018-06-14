using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtoCommerce.ImageToolsModule.Data.Models;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ImageToolsModule.Data.Reposotories
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

        ThumbnailTaskEntity[] GetThumbnailTasksByIds(string[] ids);

        ThumbnailOptionEntity[] GetThumbnailOptionsByIds(string[] ids);

        void RemoveThumbnailTasksByIds(string[] ids);

        void RemoveThumbnailOptionsByIds(string[] ids);
    }
}
