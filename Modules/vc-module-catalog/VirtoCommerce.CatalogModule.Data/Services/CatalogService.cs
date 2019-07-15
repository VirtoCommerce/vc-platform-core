using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CatalogModule.Core.Events;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CatalogModule.Data.Caching;
using VirtoCommerce.CatalogModule.Data.Model;
using VirtoCommerce.CatalogModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CatalogModule.Data.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IPlatformMemoryCache _platformMemoryCache;
        private readonly Func<ICatalogRepository> _repositoryFactory;
        private readonly IEventPublisher _eventPublisher;
        private readonly AbstractValidator<IHasProperties> _hasPropertyValidator;

        public CatalogService(Func<ICatalogRepository> catalogRepositoryFactory,
                                  IEventPublisher eventPublisher, IPlatformMemoryCache platformMemoryCache, AbstractValidator<IHasProperties> hasPropertyValidator)
        {
            _repositoryFactory = catalogRepositoryFactory;
            _eventPublisher = eventPublisher;
            _platformMemoryCache = platformMemoryCache;
            _hasPropertyValidator = hasPropertyValidator;
        }

        #region ICatalogService Members

        public virtual async Task<Catalog[]> GetByIdsAsync(string[] catalogIds, string responseGroup = null)
        {
            var catalogResponseGroup = EnumUtility.SafeParseFlags(responseGroup, CatalogResponseGroup.Full);

            var result = new List<Catalog>();
            var preloadedCatalogsByIdDict = await PreloadCatalogsAsync();
            foreach (var catalogId in catalogIds.Where(x => x != null))
            {
                var catalog = preloadedCatalogsByIdDict[catalogId];
                if (catalog != null)
                {
                    catalog = catalog.Clone() as Catalog;
                    //Reduce details according to response group
                    catalog.ReduceDetails(catalogResponseGroup.ToString());
                    result.Add(catalog);
                }
            }
            return result.ToArray();        
        }     

        public virtual async Task SaveChangesAsync(Catalog[] catalogs)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<Catalog>>();

            using (var repository = _repositoryFactory())
            {
                await ValidateCatalogPropertiesAsync(catalogs);
                var dbExistEntities = await repository.GetCatalogsByIdsAsync(catalogs.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
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
                await _eventPublisher.Publish(new CatalogChangingEvent(changedEntries));
                //Save changes in database
                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();
                await _eventPublisher.Publish(new CatalogChangedEvent(changedEntries));

                //Reset cached catalogs and catalogs
                CatalogCacheRegion.ExpireRegion();
            }
        }

        public virtual async Task DeleteAsync(string[] catalogIds)
        {
            using (var repository = _repositoryFactory())
            {
                var catalogs = await GetByIdsAsync(catalogIds);
                if (!catalogs.IsNullOrEmpty())
                {
                    var changedEntries = catalogs.Select(x => new GenericChangedEntry<Catalog>(x, EntryState.Deleted));
                    await _eventPublisher.Publish(new CatalogChangingEvent(changedEntries));

                    await repository.RemoveCatalogsAsync(catalogs.Select(m => m.Id).ToArray());
                    await repository.UnitOfWork.CommitAsync();

                    await _eventPublisher.Publish(new CatalogChangedEvent(changedEntries));
                }
            }
            CatalogCacheRegion.ExpireRegion();
        }

        #endregion

        protected virtual Task<IDictionary<string, Catalog>> PreloadCatalogsAsync()
        {
            var cacheKey = CacheKey.With(GetType(), "AllCatalogs");
            return _platformMemoryCache.GetOrCreateExclusive(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CatalogCacheRegion.CreateChangeToken());
                CatalogEntity[] entities;
                using (var repository = _repositoryFactory())
                {
                    //Optimize performance and CPU usage
                    repository.DisableChangesTracking();

                    var ids = await repository.Catalogs.Select(x => x.Id).ToArrayAsync();
                    entities = await repository.GetCatalogsByIdsAsync(ids);
                }

                var result = entities.Select(x => x.ToModel(AbstractTypeFactory<Catalog>.TryCreateInstance()))
                                     .ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase)
                                     .WithDefaultValue(null);

                LoadDependencies(result.Values, result);
                return result;
            });
        }

        protected virtual void LoadDependencies(IEnumerable<Catalog> catalogs, IDictionary<string, Catalog> preloadedCatalogsMap)
        {
            if (catalogs == null)
            {
                throw new ArgumentNullException(nameof(catalogs));
            }
            foreach (var catalog in catalogs.Where(x => !x.IsTransient()))
            {
                var preloadedCatalog = preloadedCatalogsMap[catalog.Id];
                if (preloadedCatalog != null)
                {
                    if (catalog.Properties.IsNullOrEmpty())
                    {
                        catalog.Properties = preloadedCatalog.Properties;
                    }
                    foreach (var property in catalog.Properties)
                    {
                        property.Catalog = preloadedCatalogsMap[property.CatalogId];
                    }
                }
            }
        }

        private async Task ValidateCatalogPropertiesAsync(Catalog[] catalogs)
        {
            LoadDependencies(catalogs, await PreloadCatalogsAsync());
            foreach (var catalog in catalogs)
            {
                var validatioResult = await _hasPropertyValidator.ValidateAsync(catalog);
                if (!validatioResult.IsValid)
                {
                    throw new Exception($"Catalog properties has validation error: {string.Join(Environment.NewLine, validatioResult.Errors.Select(x => x.ToString()))}");
                }
            }
        }
    }
}
