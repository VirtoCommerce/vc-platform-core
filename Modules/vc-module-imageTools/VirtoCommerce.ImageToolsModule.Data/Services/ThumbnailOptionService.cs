using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ImageToolsModule.Core.Models;
using VirtoCommerce.ImageToolsModule.Core.Services;
using VirtoCommerce.ImageToolsModule.Data.Models;
using VirtoCommerce.ImageToolsModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ImageToolsModule.Data.Services
{
    public class ThumbnailOptionService : IThumbnailOptionService
    {
        private readonly Func<IThumbnailRepository> _thumbnailRepositoryFactory;

        public ThumbnailOptionService(Func<IThumbnailRepository> thumbnailRepositoryFactory)
        {
            _thumbnailRepositoryFactory = thumbnailRepositoryFactory;
        }

        public virtual async Task SaveOrUpdateAsync(ThumbnailOption[] options)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            using (var repository = _thumbnailRepositoryFactory())
            {
                var existOptionEntities = await repository.GetThumbnailOptionsByIdsAsync(options.Select(t => t.Id).ToArray());
                foreach (var option in options)
                {
                    var sourceOptionsEntity = AbstractTypeFactory<ThumbnailOptionEntity>.TryCreateInstance();
                    if (sourceOptionsEntity != null)
                    {
                        sourceOptionsEntity = sourceOptionsEntity.FromModel(option, pkMap);
                        var targetOptionsEntity = existOptionEntities.FirstOrDefault(x => x.Id == option.Id);
                        if (targetOptionsEntity != null)
                        {
                            sourceOptionsEntity.Patch(targetOptionsEntity);
                        }
                        else
                        {
                            repository.Add(sourceOptionsEntity);
                        }
                    }
                }

                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();
            }
        }

        public virtual async Task<ThumbnailOption[]> GetByIdsAsync(string[] ids)
        {
            using (var repository = _thumbnailRepositoryFactory())
            {
                var thumbnailOptions = await repository.GetThumbnailOptionsByIdsAsync(ids);
                return thumbnailOptions.Select(x => x.ToModel(AbstractTypeFactory<ThumbnailOption>.TryCreateInstance())).ToArray();
            }
        }

        public virtual async Task RemoveByIdsAsync(string[] ids)
        {
            using (var repository = _thumbnailRepositoryFactory())
            {
                await repository.RemoveThumbnailOptionsByIds(ids);
                await repository.UnitOfWork.CommitAsync();
            }
        }
    }
}
