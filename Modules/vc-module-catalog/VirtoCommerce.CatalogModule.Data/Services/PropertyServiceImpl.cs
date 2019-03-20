using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Data.Services
{
    public class PropertyServiceImpl : IPropertyService
    {
        private readonly Func<ICatalogRepository> _repositoryFactory;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPlatformMemoryCache _platformMemoryCache;
        private readonly ICatalogService _catalogService;

        public PropertyServiceImpl(Func<ICatalogRepository> repositoryFactory, IEventPublisher eventPublisher, IPlatformMemoryCache platformMemoryCache, ICatalogService catalogService)
        {
            _repositoryFactory = repositoryFactory;
            _eventPublisher = eventPublisher;
            _platformMemoryCache = platformMemoryCache;
            _catalogService = catalogService;
        }

        #region IPropertyService Members


        public virtual async Task<Property[]> GetByIdsAsync(string[] ids)
        {
            var preloadedProperties = await PreloadAllPropertiesAsync();

            var result = ids
                .Where(propertyId => preloadedProperties.ContainsKey(propertyId))
                .Select(propertyId => preloadedProperties[propertyId])
                .Select(x => x.Clone() as Property)
                .ToArray();

            return result;
        }

        public async Task<Property[]> GetAllCatalogPropertiesAsync(string catalogId)
        {
            var preloadedProperties = await PreloadAllCatalogProperties(catalogId);
            var result = preloadedProperties.Select(x => x.Clone() as Property).ToArray();
            return result;
        }

        public async Task<Property[]> GetAllPropertiesAsync()
        {
            var preloadedProperties = await PreloadAllPropertiesAsync();
            var result = preloadedProperties.Values.Select(x => x.Clone() as Property).ToArray();
            return result;
        }

        public virtual async Task SaveChangesAsync(Property[] properties)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<Property>>();

            using (var repository = _repositoryFactory())
            {
                TryAddPredefinedValidationRules(properties);

                var dbExistEntities = await repository.GetPropertiesByIdsAsync(properties.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray(), loadDictValues: true);
                foreach (var property in properties)
                {
                    var originalEntity = dbExistEntities.FirstOrDefault(x => x.Id == property.Id);
                    var modifiedEntity = AbstractTypeFactory<PropertyEntity>.TryCreateInstance().FromModel(property, pkMap);
                    if (originalEntity != null)
                    {
                        changedEntries.Add(new GenericChangedEntry<Property>(property, originalEntity.ToModel(AbstractTypeFactory<Property>.TryCreateInstance()), EntryState.Modified));
                        modifiedEntity.Patch(originalEntity);
                        //Force set ModifiedDate property to mark a property changed. Special for  partial update cases when property table not have changes
                        originalEntity.ModifiedDate = DateTime.UtcNow;
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                        changedEntries.Add(new GenericChangedEntry<Property>(property, EntryState.Added));
                    }
                }

                await _eventPublisher.Publish(new PropertyChangingEvent(changedEntries));

                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();
                await _eventPublisher.Publish(new PropertyChangedEvent(changedEntries));
            }

            ResetCache(properties);
        }

        public async Task DeleteAsync(string[] propertyIds)
        {
            var propeties = await GetByIdsAsync(propertyIds);
            var changedEntries = propeties.Select(p => new GenericChangedEntry<Property>(p, EntryState.Deleted)).ToList();

            using (var repository = _repositoryFactory())
            {
                var entities = await repository.GetPropertiesByIdsAsync(propertyIds);

                await _eventPublisher.Publish(new PropertyChangingEvent(changedEntries));

                foreach (var entity in entities)
                {
                    repository.Remove(entity);
                }

                await repository.UnitOfWork.CommitAsync();
                
                await _eventPublisher.Publish(new PropertyChangedEvent(changedEntries));
            }

            ResetCache(propeties);
        }

        public async Task DeletePropertyValuesByPropertyIdAsync(string propertyId)
        {
            using (var repository = _repositoryFactory())
            {
                await repository.RemoveAllPropertyValuesAsync(propertyId);
                await repository.UnitOfWork.CommitAsync();
            }
        }

        #endregion

        protected virtual void ResetCache(Property[] properties)
        {
            CatalogCacheRegion.ExpireRegion();
            CategoryCacheRegion.ExpireRegion();
            
            if (properties.Any(p => p.Type == PropertyType.Product))
            {
                ItemCacheRegion.ExpireRegion();
                ItemSearchCacheRegion.ExpireRegion();
            }
        }


        protected virtual Task<Dictionary<string, Property>> PreloadAllPropertiesAsync()
        {
            var cacheKey = CacheKey.With(GetType(), "PreloadAllProperties");
            return _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CatalogCacheRegion.CreateChangeToken());
                using (var repository = _repositoryFactory())
                {
                    repository.DisableChangesTracking();

                    var propertyIds = await repository.Properties.Select(p => p.Id).ToArrayAsync();
                    var entities = await repository.GetPropertiesByIdsAsync(propertyIds);
                    var properties = entities.Select(p => p.ToModel(AbstractTypeFactory<Property>.TryCreateInstance())).ToArray();

                    await LoadDependenciesAsync(properties);
                    ApplyInheritanceRules(properties);

                    var result = properties.ToDictionary(p => p.Id, StringComparer.OrdinalIgnoreCase);

                    return result;
                }
            });
        }

        protected virtual Task<Property[]> PreloadAllCatalogProperties(string catalogId)
        {
            var cacheKey = CacheKey.With(GetType(), "PreloadAllCatalogProperties", catalogId);
            return _platformMemoryCache.GetOrCreateExclusive(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CatalogCacheRegion.CreateChangeToken());
                using (var repository = _repositoryFactory())
                {
                    repository.DisableChangesTracking();

                    var result = (await repository.GetAllCatalogPropertiesAsync(catalogId))
                        .GroupBy(p => p.Id, StringComparer.OrdinalIgnoreCase) // Remove duplicates
                        .Select(g => g.First())
                        .OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
                        .Select(p => p.ToModel(AbstractTypeFactory<Property>.TryCreateInstance()))
                        .ToArray();

                    return result;
                }
            });
        }

        protected virtual async Task LoadDependenciesAsync(Property[] properties)
        {
            var catalogsMap = (await _catalogService.GetCatalogsListAsync()).ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);
            foreach (var property in properties)
            {
                property.Catalog = catalogsMap.GetValueOrThrow(property.CatalogId, $"property catalog with key {property.CatalogId} not exist");
            }
        }

        protected virtual void ApplyInheritanceRules(Property[] properties)
        {

            foreach (var property in properties)
            {
                var displayNamesComparer = AnonymousComparer.Create((PropertyDisplayName x) => $"{x.LanguageCode}");
                var displayNamesForCatalogLanguages = property.Catalog.Languages.Select(x => new PropertyDisplayName { LanguageCode = x.LanguageCode }).ToList();

                //Leave display names only with catalog languages
                property.DisplayNames = property.DisplayNames.Intersect(displayNamesForCatalogLanguages, displayNamesComparer).ToList();

                //Add missed
                property.DisplayNames.AddRange(displayNamesForCatalogLanguages.Except(property.DisplayNames, displayNamesComparer));
                property.IsManageable = true;
            }
        }

        protected virtual void TryAddPredefinedValidationRules(Property[] properties)
        {
            foreach (var property in properties)
            {
                if (property.ValueType == PropertyValueType.GeoPoint)
                {
                    var geoPointValidationRule = property.ValidationRules?.FirstOrDefault(x => x.RegExp.EqualsInvariant(GeoPoint.Regexp.ToString()));
                    if (geoPointValidationRule == null)
                    {
                        if (property.ValidationRules == null)
                        {
                            property.ValidationRules = new List<PropertyValidationRule>();
                        }
                        property.ValidationRules.Add(new PropertyValidationRule { RegExp = GeoPoint.Regexp.ToString() });
                    }
                }
            }
        }
    }
}
