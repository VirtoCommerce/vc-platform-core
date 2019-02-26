using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CatalogModule.Core2.Events;
using VirtoCommerce.CatalogModule.Core2.Model;
using VirtoCommerce.CatalogModule.Core2.Services;
using VirtoCommerce.CatalogModule.Data2.Caching;
using VirtoCommerce.CatalogModule.Data2.Model;
using VirtoCommerce.CatalogModule.Data2.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CatalogModule.Data2.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IPlatformMemoryCache _memoryCache;
        private readonly AbstractValidator<IHasProperties> _hasPropertyValidator;
        private readonly Func<ICatalogRepository> _repositoryFactory;
        private readonly IEventPublisher _eventPublisher;

        public CatalogService(Func<ICatalogRepository> catalogRepositoryFactory, IPlatformMemoryCache cacheManager, AbstractValidator<IHasProperties> hasPropertyValidator, IEventPublisher eventPublisher)
        {
            _repositoryFactory = catalogRepositoryFactory;
            _memoryCache = cacheManager;
            _hasPropertyValidator = hasPropertyValidator;
            _eventPublisher = eventPublisher;
        }

        #region ICatalogService Members

        public virtual IEnumerable<Catalog> GetAllCatalogs()
        {
            return PreloadCatalogs().Select(x => x.Value.Clone() as Catalog)
                                          .ToArray();
        }

        public virtual IEnumerable<Catalog> GetByIds(IEnumerable<string> catalogIds)
        {
            //Clone required because client code may change resulting objects
            var result = PreloadCatalogs().Join(catalogIds, pair => pair.Key, id => id, (pair, id) => pair.Value)
                                          .Select(x => x.Clone() as Catalog)
                                          .ToArray();
            return result;
        }

        public virtual void SaveChanges(IEnumerable<Catalog> catalogs)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<Catalog>>();

            using (var repository = _repositoryFactory())
            {
                ValidateCatalogProperties(catalogs);
                var dbExistEntities = repository.GetCatalogsByIds(catalogs.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var catalog in catalogs)
                {
                    var originalEntity = dbExistEntities.FirstOrDefault(x => x.Id == catalog.Id);
                    var modifiedEntity = AbstractTypeFactory<CatalogEntity>.TryCreateInstance().FromModel(catalog, pkMap);
                    if (originalEntity != null)
                    {
                        changedEntries.Add(new GenericChangedEntry<Catalog>(catalog, originalEntity.ToModel(AbstractTypeFactory<Catalog>.TryCreateInstance()), EntryState.Modified));
                        modifiedEntity.Patch(originalEntity);
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                        changedEntries.Add(new GenericChangedEntry<Catalog>(catalog, EntryState.Added));
                    }
                }
                //Raise domain events
                _eventPublisher.Publish(new CatalogChangingEvent(changedEntries));
                //Save changes in database
                repository.UnitOfWork.Commit();
                pkMap.ResolvePrimaryKeys();
                _eventPublisher.Publish(new CatalogChangedEvent(changedEntries));

                //Reset cached catalogs and catalogs
                CatalogCacheRegion.ExpireRegion();
            }
        }

        public void Delete(IEnumerable<string> catalogIds)
        {
            var catalogs = GetByIds(catalogIds);
            //Raise domain events before deletion
            var changedEntries = catalogs.Select(x => new GenericChangedEntry<Catalog>(x, EntryState.Deleted));
            _eventPublisher.Publish(new CatalogChangingEvent(changedEntries));
            using (var repository = _repositoryFactory())
            {
                //TODO:  raise events on catalog deletion
                repository.RemoveCatalogs(catalogIds.ToArray());
                repository.UnitOfWork.Commit();
                //Reset cached catalogs and catalogs
                CatalogCacheRegion.ExpireRegion();
            }
            _eventPublisher.Publish(new CatalogChangedEvent(changedEntries));
        }
        #endregion

        protected virtual Dictionary<string, Catalog> PreloadCatalogs()
        {
            var cacheKey = CacheKey.With(GetType(), "PreloadCatalogs");
            return _memoryCache.GetOrCreateExclusive(cacheKey, (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CatalogCacheRegion.CreateChangeToken());
                CatalogEntity[] entities;
                using (var repository = _repositoryFactory())
                {
                    //Optimize performance and CPU usage
                    repository.DisableChangesTracking();
                    entities = repository.GetCatalogsByIds(repository.Catalogs.Select(x => x.Id).ToArray());
                }

                var result = entities.Select(x => x.ToModel(AbstractTypeFactory<Catalog>.TryCreateInstance()))
                                     .ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);

                LoadDependencies(result.Values, result);
                return result;
            });
        }

        protected virtual void LoadDependencies(IEnumerable<Catalog> catalogs, Dictionary<string, Catalog> preloadedCatalogsMap)
        {
            if (catalogs == null)
            {
                throw new ArgumentNullException(nameof(catalogs));
            }
            foreach (var catalog in catalogs.Where(x => !x.IsTransient()))
            {
                if (preloadedCatalogsMap.TryGetValue(catalog.Id, out var preloadedCatalog))
                {
                    catalog.Properties = preloadedCatalog.Properties;
                    foreach (var property in catalog.Properties)
                    {
                        property.Catalog = preloadedCatalogsMap[property.CatalogId];
                        property.ActualizeValues();
                    }
                }
            }
        }

        private void ValidateCatalogProperties(IEnumerable<Catalog> catalogs)
        {
            LoadDependencies(catalogs, PreloadCatalogs());
            foreach (var catalog in catalogs)
            {
                var validatioResult = _hasPropertyValidator.Validate(catalog);
                if (!validatioResult.IsValid)
                    throw new Exception($"Catalog properties has validation error: {string.Join(Environment.NewLine, validatioResult.Errors.Select(x => x.ToString()))}");
            }
        }
    }
}
