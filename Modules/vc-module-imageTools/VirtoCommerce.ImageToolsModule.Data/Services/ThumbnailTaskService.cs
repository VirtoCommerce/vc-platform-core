using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ImageToolsModule.Core.Models;
using VirtoCommerce.ImageToolsModule.Core.Services;
using VirtoCommerce.ImageToolsModule.Data.Models;
using VirtoCommerce.ImageToolsModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ImageToolsModule.Data.Services
{
    public class ThumbnailTaskService : IThumbnailTaskService
    {
        private readonly Func<IThumbnailRepository> _thumbnailRepositoryFactory;

        public ThumbnailTaskService(Func<IThumbnailRepository> thumbnailRepositoryFactory)
        {
            _thumbnailRepositoryFactory = thumbnailRepositoryFactory;
        }

        public virtual async Task SaveChangesAsync(ICollection<ThumbnailTask> tasks)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            using (var repository = _thumbnailRepositoryFactory())
            {
                var existPlanEntities = await repository.GetThumbnailTasksByIdsAsync(tasks.Select(t => t.Id).ToArray());
                foreach (var task in tasks)
                {
                    var sourceTaskEntity = AbstractTypeFactory<ThumbnailTaskEntity>.TryCreateInstance();
                    if (sourceTaskEntity != null)
                    {
                        sourceTaskEntity = sourceTaskEntity.FromModel(task, pkMap);
                        var targetTaskEntity = existPlanEntities.FirstOrDefault(x => x.Id == task.Id);
                        if (targetTaskEntity != null)
                        {
                            sourceTaskEntity.Patch(targetTaskEntity);
                        }
                        else
                        {
                            repository.Add(sourceTaskEntity);
                        }
                    }
                }

                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();
            }
        }

        public virtual async Task RemoveByIdsAsync(string[] ids)
        {
            using (var repository = _thumbnailRepositoryFactory())
            {
                await repository.RemoveThumbnailTasksByIdsAsync(ids);
                await repository.UnitOfWork.CommitAsync();
            }
        }

        public virtual async Task<ThumbnailTask[]> GetByIdsAsync(string[] ids)
        {
            using (var repository = _thumbnailRepositoryFactory())
            {
                var thumbnailTasks = await repository.GetThumbnailTasksByIdsAsync(ids);
                return thumbnailTasks.Select(x => x.ToModel(AbstractTypeFactory<ThumbnailTask>.TryCreateInstance())).ToArray();
            }
        }
    }
}
