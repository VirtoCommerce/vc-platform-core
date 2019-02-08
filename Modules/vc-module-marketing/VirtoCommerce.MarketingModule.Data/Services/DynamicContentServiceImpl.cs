using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.MarketingModule.Core.Model.DynamicContent;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.MarketingModule.Data.Model;
using VirtoCommerce.MarketingModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.MarketingModule.Data.Services
{
    public class DynamicContentServiceImpl : IDynamicContentService
    {
        private readonly Func<IMarketingRepository> _repositoryFactory;
        private readonly IDynamicPropertyService _dynamicPropertyService;

        public DynamicContentServiceImpl(Func<IMarketingRepository> repositoryFactory, IDynamicPropertyService dynamicPropertyService)
        {
            _repositoryFactory = repositoryFactory;
            _dynamicPropertyService = dynamicPropertyService;
        }

        #region IDynamicContentService Members

        #region DynamicContentItem methods
        public async Task<DynamicContentItem[]> GetContentItemsByIdsAsync(string[] ids)
        {
            DynamicContentItem[] retVal = null;
            using (var repository = _repositoryFactory())
            {
                retVal = (await repository.GetContentItemsByIdsAsync(ids)).Select(x => x.ToModel(AbstractTypeFactory<DynamicContentItem>.TryCreateInstance())).ToArray();
            }

            if (retVal != null)
            {
                await _dynamicPropertyService.LoadDynamicPropertyValuesAsync(retVal);
            }

            return retVal;
        }

        public async Task SaveContentItemsAsync(DynamicContentItem[] items)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            using (var repository = _repositoryFactory())
            {
                var existEntities = await repository.GetContentItemsByIdsAsync(items.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var item in items)
                {
                    var sourceEntity = AbstractTypeFactory<DynamicContentItemEntity>.TryCreateInstance();
                    if (sourceEntity != null)
                    {
                        sourceEntity = sourceEntity.FromModel(item, pkMap);
                        var targetEntity = existEntities.FirstOrDefault(x => x.Id == item.Id);
                        if (targetEntity != null)
                        {
                            sourceEntity.Patch(targetEntity);
                        }
                        else
                        {
                            repository.Add(sourceEntity);
                        }
                    }
                }
                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();
            }

            //TODO move to handler
            //foreach (var item in items)
            //{
            //    _dynamicPropertyService.SaveDynamicPropertyValues(item);
            //}
        }

        public async Task DeleteContentItems(string[] ids)
        {
            var items = await GetContentItemsByIdsAsync(ids);
            //TODO move to handler
            //foreach (var item in items)
            //{
            //    _dynamicPropertyService.DeleteDynamicPropertyValues(item);
            //}
            using (var repository = _repositoryFactory())
            {
                await repository.RemoveContentItemsAsync(ids);
                await repository.UnitOfWork.CommitAsync();
            }
        }
        #endregion

        #region DynamicContentPlace methods
        public DynamicContentPlace[] GetPlacesByIds(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                return repository.GetContentPlacesByIds(ids).Select(x => x.ToModel(AbstractTypeFactory<DynamicContentPlace>.TryCreateInstance())).ToArray();
            }
        }

        public void SavePlaces(DynamicContentPlace[] places)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            using (var repository = _repositoryFactory())
            {
                var existEntities = repository.GetContentPlacesByIds(places.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var place in places)
                {
                    var sourceEntity = AbstractTypeFactory<DynamicContentPlaceEntity>.TryCreateInstance();
                    if (sourceEntity != null)
                    {
                        sourceEntity = sourceEntity.FromModel(place, pkMap);
                        var targetEntity = existEntities.FirstOrDefault(x => x.Id == place.Id);
                        if (targetEntity != null)
                        {
                            changeTracker.Attach(targetEntity);
                            sourceEntity.Patch(targetEntity);
                        }
                        else
                        {
                            repository.Add(sourceEntity);
                        }
                    }
                }
                CommitChanges(repository);
                pkMap.ResolvePrimaryKeys();
            }
        }

        public void DeletePlaces(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                repository.RemovePlaces(ids);
                CommitChanges(repository);
            }
        }
        #endregion

        #region DynamicContentPublication methods
        public DynamicContentPublication[] GetPublicationsByIds(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                return repository.GetContentPublicationsByIds(ids).Select(x => x.ToModel(AbstractTypeFactory<DynamicContentPublication>.TryCreateInstance())).ToArray();
            }
        }

        public void SavePublications(DynamicContentPublication[] publications)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            using (var repository = _repositoryFactory())
            using (var changeTracker = GetChangeTracker(repository))
            {
                var existEntities = repository.GetContentPublicationsByIds(publications.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var publication in publications)
                {
                    var sourceEntity = AbstractTypeFactory<DynamicContentPublishingGroupEntity>.TryCreateInstance();
                    if (sourceEntity != null)
                    {
                        sourceEntity = sourceEntity.FromModel(publication, pkMap);
                        var targetEntity = existEntities.FirstOrDefault(x => x.Id == publication.Id);
                        if (targetEntity != null)
                        {
                            changeTracker.Attach(targetEntity);
                            sourceEntity.Patch(targetEntity);
                        }
                        else
                        {
                            repository.Add(sourceEntity);
                        }
                    }
                }
                CommitChanges(repository);
                pkMap.ResolvePrimaryKeys();
            }
        }

        public void DeletePublications(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                repository.RemoveContentPublications(ids);
                CommitChanges(repository);
            }
        }
        #endregion


        #region DynamicContentFolder methods
        public DynamicContentFolder[] GetFoldersByIds(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                return repository.GetContentFoldersByIds(ids).Select(x => x.ToModel(AbstractTypeFactory<DynamicContentFolder>.TryCreateInstance())).ToArray();
            }
        }

        public void SaveFolders(DynamicContentFolder[] folders)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            using (var repository = _repositoryFactory())
            using (var changeTracker = GetChangeTracker(repository))
            {
                var existEntities = repository.GetContentFoldersByIds(folders.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var folder in folders)
                {
                    var sourceEntity = AbstractTypeFactory<DynamicContentFolderEntity>.TryCreateInstance();
                    if (sourceEntity != null)
                    {
                        sourceEntity = sourceEntity.FromModel(folder, pkMap);
                        var targetEntity = existEntities.FirstOrDefault(x => x.Id == folder.Id);
                        if (targetEntity != null)
                        {
                            changeTracker.Attach(targetEntity);
                            sourceEntity.Patch(targetEntity);
                        }
                        else
                        {
                            repository.Add(sourceEntity);
                        }
                    }
                }
                CommitChanges(repository);
                pkMap.ResolvePrimaryKeys();
            }
        }

        public void DeleteFolders(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                repository.RemoveFolders(ids);
                CommitChanges(repository);
            }
        }
        #endregion


        #endregion
    }
}
