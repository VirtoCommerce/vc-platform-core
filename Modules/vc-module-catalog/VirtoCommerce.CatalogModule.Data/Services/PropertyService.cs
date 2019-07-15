using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CatalogModule.Core.Events;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CatalogModule.Data.Caching;
using VirtoCommerce.CatalogModule.Data.Model;
using VirtoCommerce.CatalogModule.Data.Repositories;
using VirtoCommerce.CatalogModule.Data.Validation;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Data.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly Func<ICatalogRepository> _repositoryFactory;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPlatformMemoryCache _platformMemoryCache;
        private readonly ICatalogSearchService _catalogSearchService;

        public PropertyService(
            Func<ICatalogRepository> repositoryFactory
            , IEventPublisher eventPublisher
            , IPlatformMemoryCache platformMemoryCache
            , ICatalogSearchService catalogSearchService)
        {
            _repositoryFactory = repositoryFactory;
            _eventPublisher = eventPublisher;
            _platformMemoryCache = platformMemoryCache;
            _catalogSearchService = catalogSearchService;
        }

        #region IPropertyService members
        public async Task<IEnumerable<Property>> GetByIdsAsync(IEnumerable<string> ids)
        {
            var preloadedProperties = await PreloadAllPropertiesAsync();

            var result = ids
                .Select(x => preloadedProperties[x])
                .Where(x => x != null)
                .Select(x => x.Clone()).OfType<Property>()
                .ToList();

            return result;
        }

        public async Task<IEnumerable<Property>> GetAllCatalogPropertiesAsync(string catalogId)
        {
            var preloadedProperties = await PreloadAllCatalogPropertiesAsync(catalogId);
            var result = preloadedProperties.Select(x => x.Clone()).OfType<Property>().ToArray();
            return result;
        }

        public async Task SaveChangesAsync(IEnumerable<Property> properties)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<Property>>();

            ValidateProperties(properties);

            using (var repository = _repositoryFactory())
            {
                TryAddPredefinedValidationRules(properties);

                var dbExistProperties = await repository.GetPropertiesByIdsAsync(properties.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var property in properties)
                {
                    var modifiedEntity = AbstractTypeFactory<PropertyEntity>.TryCreateInstance().FromModel(property, pkMap);
                    var originalEntity = dbExistProperties.FirstOrDefault(x => x.Id == property.Id);

                    if (originalEntity != null)
                    {
                        changedEntries.Add(new GenericChangedEntry<Property>(property, originalEntity.ToModel(AbstractTypeFactory<Property>.TryCreateInstance()), EntryState.Modified));
                        modifiedEntity.Patch(originalEntity);
                        //Force set ModifiedDate property to mark a product changed. Special for  partial update cases when product table not have changes
                        originalEntity.ModifiedDate = DateTime.UtcNow;
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                        changedEntries.Add(new GenericChangedEntry<Property>(property, EntryState.Added));
                    }
                }

                //Raise domain events
                await _eventPublisher.Publish(new PropertyChangingEvent(changedEntries));
                //Save changes in database
                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();
                await _eventPublisher.Publish(new PropertyChangedEvent(changedEntries));

                //Reset catalog cache
                CatalogCacheRegion.ExpireRegion();
            }
        }

        public async Task DeleteAsync(IEnumerable<string> ids, bool doDeleteValues = false)
        {
            using (var repository = _repositoryFactory())
            {
                var entities = await repository.GetPropertiesByIdsAsync(ids.ToArray());
                //Raise domain events before deletion
                var changedEntries = entities.Select(x => new GenericChangedEntry<Property>(x.ToModel(AbstractTypeFactory<Property>.TryCreateInstance()), EntryState.Deleted));

                await _eventPublisher.Publish(new PropertyChangingEvent(changedEntries));

                foreach (var entity in entities)
                {
                    repository.Remove(entity);
                    if (doDeleteValues)
                    {
                        await repository.RemoveAllPropertyValuesAsync(entity.Id);
                    }
                }
                await repository.UnitOfWork.CommitAsync();
                await _eventPublisher.Publish(new PropertyChangedEvent(changedEntries));
                //Reset catalog cache
                CatalogCacheRegion.ExpireRegion();
            }
        }
        #endregion

        protected virtual async Task<IDictionary<string, Property>> PreloadAllPropertiesAsync()
        {
            var cacheKey = CacheKey.With(GetType(), "PreloadAllProperties");
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
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

                    var result = properties.ToDictionary(p => p.Id, StringComparer.OrdinalIgnoreCase).WithDefaultValue(null);
                    return result;
                }
            });
        }

        protected virtual async Task<Property[]> PreloadAllCatalogPropertiesAsync(string catalogId)
        {
            var cacheKey = CacheKey.With(GetType(), "PreloadAllCatalogProperties", catalogId);
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
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
            var catalogsByIdDict = ((await _catalogSearchService.SearchCatalogsAsync(new Core.Model.Search.CatalogSearchCriteria { Take = int.MaxValue })).Results).ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase)
                                                                           .WithDefaultValue(null);
            foreach (var property in properties)
            {
                property.Catalog = catalogsByIdDict[property.CatalogId];
            }
        }

        protected virtual void ApplyInheritanceRules(Property[] properties)
        {
            foreach (var property in properties)
            {
                property.TryInheritFrom(property.Catalog);
            }
        }

        protected virtual void TryAddPredefinedValidationRules(IEnumerable<Property> properties)
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

        protected virtual void ValidateProperties(IEnumerable<Property> properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }
            //Validate products
            var validator = AbstractTypeFactory<PropertyValidator>.TryCreateInstance();
            foreach (var property in properties)
            {
                validator.ValidateAndThrow(property);
            }
        }
    }
}

