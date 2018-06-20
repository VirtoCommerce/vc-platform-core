using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using VirtoCommerce.ImageToolsModule.Core.Models;
using VirtoCommerce.ImageToolsModule.Core.Services;
using VirtoCommerce.ImageToolsModule.Data.Models;
using VirtoCommerce.ImageToolsModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ImageToolsModule.Data.Services
{
    public class ThumbnailTaskService : ServiceBase, IThumbnailTaskService
    {
        private readonly Func<IThumbnailRepository> _thumbnailRepositoryFactory;

        public ThumbnailTaskService(Func<IThumbnailRepository> thumbnailRepositoryFactory)
        {
            _thumbnailRepositoryFactory = thumbnailRepositoryFactory;
        }

        public async Task SaveOrUpdateAsync(ICollection<ThumbnailTask> tasks)
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

                repository.UnitOfWork.Commit();
                pkMap.ResolvePrimaryKeys();
            }
        }

        public async Task RemoveByIdsAsync(string[] ids)
        {
            using (var repository = _thumbnailRepositoryFactory())
            {
                await repository.RemoveThumbnailTasksByIdsAsync(ids);
                repository.UnitOfWork.Commit();
            }
        }

        public async Task<ThumbnailTask[]> GetByIdsAsync(string[] ids)
        {
            using (var repository = _thumbnailRepositoryFactory())
            {
                var thumbnailTasks = await repository.GetThumbnailTasksByIdsAsync(ids);
                return thumbnailTasks.Select(x => x.ToModel(AbstractTypeFactory<ThumbnailTask>.TryCreateInstance())).ToArray();
            }
        }
    }
}
