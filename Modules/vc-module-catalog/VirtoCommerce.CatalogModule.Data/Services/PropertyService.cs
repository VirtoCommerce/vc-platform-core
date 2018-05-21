using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CatalogModule.Core.Events;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CatalogModule.Data.Caching;
using VirtoCommerce.CatalogModule.Data.Model;
using VirtoCommerce.CatalogModule.Data.Repositories;
using VirtoCommerce.CatalogModule.Data.Validation;
using VirtoCommerce.Domain.Search;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CatalogModule.Data.Services
{
    public class PropertyService : ServiceBase, IPropertyService
    {
        private readonly Func<ICatalogRepository> _repositoryFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly ICatalogService _catalogService;
        private readonly IEventPublisher _eventPublisher;
        public PropertyService(Func<ICatalogRepository> repositoryFactory, IMemoryCache memoryCache, ICatalogService catalogService, IEventPublisher eventPublisher)
        {
            _repositoryFactory = repositoryFactory;
            _memoryCache = memoryCache;
            _catalogService = catalogService;
            _eventPublisher = eventPublisher;
        }

        #region IPropertyService members
        public IEnumerable<Property> GetByIds(IEnumerable<string> ids)
        {
            var preloadedProperties = PreloadAllProperties();

            var result = ids
                .Where(propertyId => preloadedProperties.ContainsKey(propertyId))
                .Select(propertyId => preloadedProperties[propertyId])
                .Select(x => x.Clone()).OfType<Property>()
                .ToList();

            return result;
        }

        public void SaveChanges(IEnumerable<Property> properties)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<ChangedEntry<Property>>();

            ValidateProperties(properties);

            using (var repository = _repositoryFactory())
            using (var changeTracker = GetChangeTracker(repository))
            {
                TryAddPredefinedValidationRules(properties);

                var dbExistProperties = repository.GetPropertiesByIds(properties.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var property in properties)
                {
                    var modifiedEntity = AbstractTypeFactory<PropertyEntity>.TryCreateInstance().FromModel(property, pkMap);
                    var originalEntity = dbExistProperties.FirstOrDefault(x => x.Id == property.Id);

                    if (originalEntity != null)
                    {
                        changeTracker.Attach(originalEntity);
                        modifiedEntity.Patch(originalEntity);
                        //Force set ModifiedDate property to mark a product changed. Special for  partial update cases when product table not have changes
                        originalEntity.ModifiedDate = DateTime.UtcNow;
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                    }
                }

                //Raise domain events
                _eventPublisher.Publish(new GenericCatalogChangingEvent<Property>(changedEntries));
                //Save changes in database
                repository.UnitOfWork.Commit();
                pkMap.ResolvePrimaryKeys();
                _eventPublisher.Publish(new GenericCatalogChangedEvent<Property>(changedEntries));

                //Reset catalog cache
                CatalogCacheRegion.ExpireRegion();
            }
        }

        public void Delete(IEnumerable<string> ids, bool doDeleteValues = false)
        {
            using (var repository = _repositoryFactory())
            {
                var entities = repository.GetPropertiesByIds(ids.ToArray());
                foreach (var entity in entities)
                {
                    repository.Remove(entity);
                    if (doDeleteValues)
                    {
                        repository.RemoveAllPropertyValues(entity.Id);
                    }
                }
                repository.UnitOfWork.Commit();
                //Reset catalog cache
                CatalogCacheRegion.ExpireRegion();
            }
        }
        #endregion

    
        protected virtual void LoadDependencies(Property[] properties)
        {
            foreach (var property in properties)
            {
                property.Catalog = _catalogService.GetByIds(new[] { property.CatalogId }).First();
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

      
        protected virtual IDictionary<string, Property> PreloadAllProperties()
        {
            var cacheKey = CacheKey.With(GetType(), "PreloadAllProperties");
            return _memoryCache.GetOrCreateExclusive(cacheKey, (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CatalogCacheRegion.CreateChangeToken());
                using (var repository = _repositoryFactory())
                {
                    repository.DisableChangesTracking();

                    var propertyIds = repository.Properties.Select(p => p.Id).ToArray();
                    var entities = repository.GetPropertiesByIds(propertyIds);
                    var properties = entities.Select(p => p.ToModel(AbstractTypeFactory<Property>.TryCreateInstance())).ToArray();

                    LoadDependencies(properties);
                    ApplyInheritanceRules(properties);
                    var result = properties.ToDictionary(p => p.Id, StringComparer.OrdinalIgnoreCase);
                    return result;
                }
            });
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
