using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.ImageToolsModule.Data.Models;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.ImageToolsModule.Data.Repositories
{
    public class ThumbnailRepository : DbContextRepositoryBase<ThumbnailDbContext>, IThumbnailRepository
    {
        public ThumbnailRepository(ThumbnailDbContext dbContext)
            : base(dbContext)
        {
        }

        public IQueryable<ThumbnailTaskEntity> ThumbnailTasks => DbContext.Set<ThumbnailTaskEntity>();

        public IQueryable<ThumbnailOptionEntity> ThumbnailOptions => DbContext.Set<ThumbnailOptionEntity>();

        public async Task<ThumbnailTaskEntity[]> GetThumbnailTasksByIdsAsync(string[] ids)
        {
            return await ThumbnailTasks
                .Include(t => t.ThumbnailTaskOptions)
                .ThenInclude(o => o.ThumbnailOption)
                .Where(t => ids.Contains(t.Id))
                .ToArrayAsync();
        }

        public async Task<ThumbnailOptionEntity[]> GetThumbnailOptionsByIdsAsync(string[] ids)
        {
            return await ThumbnailOptions.Where(x => ids.Contains(x.Id)).ToArrayAsync();
        }

        public async Task RemoveThumbnailTasksByIdsAsync(string[] ids)
        {
            var taskEntities = await GetThumbnailTasksByIdsAsync(ids);

            foreach (var taskEntity in taskEntities)
            {
                Remove(taskEntity);
            }
        }

        public async Task RemoveThumbnailOptionsByIds(string[] ids)
        {
            var optionEntities = await GetThumbnailOptionsByIdsAsync(ids);

            foreach (var optionEntity in optionEntities)
            {
                Remove(optionEntity);
            }
        }
    }
}
