using System;
using System.Linq;
using CacheManager.Core;
using VirtoCommerce.CatalogModule.Data.Model;
using VirtoCommerce.CatalogModule.Data.Repositories;
using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Domain.Catalog.Model.Search;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CatalogModule.Data.Services
{
    public class PropertyDictionaryItemService : ServiceBase, IProperyDictionaryItemService, IProperyDictionaryItemSearchService
    {
        private readonly Func<ICatalogRepository> _repositoryFactory;
        private readonly ICacheManager<object> _cacheManager;

        public PropertyDictionaryItemService(Func<ICatalogRepository> repositoryFactory, ICacheManager<object> cacheManager)
        {
            _repositoryFactory = repositoryFactory;
            _cacheManager = cacheManager;
        }

        public PropertyDictionaryItem[] GetByIds(string[] ids)
        {
            PropertyDictionaryItem[] result;

            using (var repository = _repositoryFactory())
            {
                //Optimize performance and CPU usage
                repository.DisableChangesTracking();

                result = repository.GetPropertyDictionaryItemsByIds(ids)
                                   .Select(x => x.ToModel(AbstractTypeFactory<PropertyDictionaryItem>.TryCreateInstance()))
                                   .ToArray();
            }
            return result;
        }

        public void SaveChanges(PropertyDictionaryItem[] dictItems)
        {
            if (dictItems == null)
            {
                throw new ArgumentNullException(nameof(dictItems));
            }

            var pkMap = new PrimaryKeyResolvingMap();
            using (var repository = _repositoryFactory())
            using (var changeTracker = GetChangeTracker(repository))
            {
                var dbExistEntities = repository.GetPropertyDictionaryItemsByIds(dictItems.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var dictItem in dictItems)
                {
                    var originalEntity = dbExistEntities.FirstOrDefault(x => x.Id == dictItem.Id);
                    var modifiedEntity = AbstractTypeFactory<PropertyDictionaryItemEntity>.TryCreateInstance().FromModel(dictItem, pkMap);
                    if (originalEntity != null)
                    {
                        changeTracker.Attach(originalEntity);
                        modifiedEntity.Patch(originalEntity);
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                    }
                }
                CommitChanges(repository);
                pkMap.ResolvePrimaryKeys();
            }
            ResetCache();
        }

        public void Delete(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                var dbEntities = repository.GetPropertyDictionaryItemsByIds(ids);

                foreach (var dbEntity in dbEntities)
                {
                    repository.Remove(dbEntity);
                }
                CommitChanges(repository);
            }
            ResetCache();
        }

        protected virtual void ResetCache()
        {
            _cacheManager.ClearRegion(CatalogConstants.DictionaryItemsCacheRegion);
        }

        public GenericSearchResult<PropertyDictionaryItem> Search(PropertyDictionaryItemSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            return _cacheManager.Get($"PropertyDictionaryItemService.Search-{criteria.GetCacheKey()}", CatalogConstants.DictionaryItemsCacheRegion, () =>
            {
                using (var repository = _repositoryFactory())
                {
                    //Optimize performance and CPU usage
                    repository.DisableChangesTracking();

                    var result = new GenericSearchResult<PropertyDictionaryItem>();

                    var query = repository.PropertyDictionaryItems;
                    if (!criteria.PropertyIds.IsNullOrEmpty())
                    {
                        query = query.Where(x => criteria.PropertyIds.Contains(x.PropertyId));
                    }
                    if (!string.IsNullOrEmpty(criteria.SearchPhrase))
                    {
                        query = query.Where(x => x.Alias.Contains(criteria.SearchPhrase));
                    }

                    var sortInfos = criteria.SortInfos;
                    if (sortInfos.IsNullOrEmpty())
                    {
                        sortInfos = new[] {
                            new SortInfo { SortColumn = "SortOrder", SortDirection = SortDirection.Ascending },
                            new SortInfo { SortColumn = "Alias", SortDirection = SortDirection.Ascending }
                        };
                    }

                    query = query.OrderBySortInfos(sortInfos);

                    result.TotalCount = query.Count();
                    var ids = query.Skip(criteria.Skip).Take(criteria.Take).Select(x => x.Id).ToArray();
                    result.Results = GetByIds(ids).AsQueryable().OrderBySortInfos(sortInfos).ToList();
                    return result;
                }
            });
        }
    }
}

