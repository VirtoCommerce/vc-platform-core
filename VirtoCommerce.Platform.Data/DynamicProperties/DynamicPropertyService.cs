using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Model;
using VirtoCommerce.Platform.Data.Repositories;

namespace VirtoCommerce.Platform.Data.DynamicProperties
{
    public class DynamicPropertyService : IDynamicPropertyService, IDynamicPropertyRegistrar
    {
        private readonly Func<IPlatformRepository> _repositoryFactory;
        private readonly IPlatformMemoryCache _memoryCache;

        public DynamicPropertyService(Func<IPlatformRepository> repositoryFactory, IPlatformMemoryCache memoryCache)
        {
            _repositoryFactory = repositoryFactory;
            _memoryCache = memoryCache;
        }

        #region IDynamicPropertyRegistrar methods
        public virtual IEnumerable<string> AllRegisteredTypeNames
        {
            get
            {
                return AbstractTypeFactory<IHasDynamicProperties>.AllTypeInfos.Select(n => n.Type.FullName);
            }
        }

        public virtual void RegisterType<T>() where T : IHasDynamicProperties
        {
            if (!AbstractTypeFactory<IHasDynamicProperties>.AllTypeInfos.Any(t => t.Type == typeof(T)))
            {
                AbstractTypeFactory<IHasDynamicProperties>.RegisterType<T>();
            }
        }

        #endregion
        #region IDynamicPropertyService Members

        public virtual async Task<DynamicProperty[]> GetDynamicPropertiesAsync(string[] ids)
        {
            var cacheKey = CacheKey.With(GetType(), "GetDynamicPropertiesAsync", string.Join("-", ids));
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                //Add cache  expiration token
                cacheEntry.AddExpirationToken(DynamicPropertiesCacheRegion.CreateChangeToken());
                using (var repository = _repositoryFactory())
                {
                    //Optimize performance and CPU usage
                    repository.DisableChangesTracking();

                    var result = await repository.GetDynamicPropertiesByIdsAsync(ids);
                    return result.Select(x => x.ToModel(AbstractTypeFactory<DynamicProperty>.TryCreateInstance())).ToArray();
                }
            });
        }


        public virtual async Task<DynamicPropertyDictionaryItem[]> GetDynamicPropertyDictionaryItemsAsync(string[] ids)
        {
            var cacheKey = CacheKey.With(GetType(), "GetDynamicPropertyDictionaryItemsAsync", string.Join("-", ids));
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(DynamicPropertiesCacheRegion.CreateChangeToken());
                using (var repository = _repositoryFactory())
                {
                    //Optimize performance and CPU usage
                    repository.DisableChangesTracking();

                    var result = await repository.GetDynamicPropertyDictionaryItemByIdsAsync(ids);
                    return result.Select(x => x.ToModel(AbstractTypeFactory<DynamicPropertyDictionaryItem>.TryCreateInstance())).ToArray();
                }
            });
        }

        public virtual async Task<DynamicProperty[]> SaveDynamicPropertiesAsync(DynamicProperty[] properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }
            var pkMap = new PrimaryKeyResolvingMap();
            using (var repository = _repositoryFactory())
            {
                var dbExistProperties = (await repository.GetDynamicPropertiesForTypesAsync(properties.Select(x => x.ObjectType).Distinct().ToArray())).ToList();
                foreach (var property in properties)
                {
                    var originalEntity = dbExistProperties.FirstOrDefault(x => property.IsTransient() ? x.Name.EqualsInvariant(property.Name) && x.ObjectType.EqualsInvariant(property.ObjectType) : x.Id.EqualsInvariant(property.Id));
                    var modifiedEntity = AbstractTypeFactory<DynamicPropertyEntity>.TryCreateInstance().FromModel(property, pkMap);
                    if (originalEntity != null)
                    {
                        modifiedEntity.Patch(originalEntity);
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                    }
                }
                repository.UnitOfWork.Commit();
                pkMap.ResolvePrimaryKeys();

                DynamicPropertiesCacheRegion.ExpireRegion();
            }
            return properties;
        }

        public virtual async Task DeleteDynamicPropertiesAsync(string[] propertyIds)
        {
            if (propertyIds == null)
            {
                throw new ArgumentNullException(nameof(propertyIds));
            }

            using (var repository = _repositoryFactory())
            {
                var properties = repository.DynamicProperties.Where(p => propertyIds.Contains(p.Id))
                                           .ToList();

                foreach (var property in properties)
                {
                    repository.Remove(property);
                }

                await repository.UnitOfWork.CommitAsync();

                DynamicPropertiesCacheRegion.ExpireRegion();
            }
        }


        public virtual async Task SaveDictionaryItemsAsync(DynamicPropertyDictionaryItem[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            using (var repository = _repositoryFactory())
            {
                var dbExistItems = await repository.GetDynamicPropertyDictionaryItemByIdsAsync(items.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var item in items)
                {
                    var originalEntity = dbExistItems.FirstOrDefault(x => x.Id == item.Id);
                    var modifiedEntity = AbstractTypeFactory<DynamicPropertyDictionaryItemEntity>.TryCreateInstance().FromModel(item);
                    if (originalEntity != null)
                    {
                        modifiedEntity.Patch(originalEntity);
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                    }
                }
                await repository.UnitOfWork.CommitAsync();

                DynamicPropertiesCacheRegion.ExpireRegion();
            }

        }

        public virtual async Task DeleteDictionaryItemsAsync(string[] itemIds)
        {
            if (itemIds == null)
            {
                throw new ArgumentNullException(nameof(itemIds));
            }

            using (var repository = _repositoryFactory())
            {
                var items = repository.DynamicPropertyDictionaryItems
                    .Where(v => itemIds.Contains(v.Id))
                    .ToList();

                foreach (var item in items)
                {
                    repository.Remove(item);
                }

                await repository.UnitOfWork.CommitAsync();

                DynamicPropertiesCacheRegion.ExpireRegion();
            }
        }

        public virtual async Task LoadDynamicPropertyValuesAsync(params IHasDynamicProperties[] owners)
        {
            if (owners == null)
            {
                throw new ArgumentNullException(nameof(owners));
            }
            //TODO: Add caching

            var propOwners = owners.SelectMany(x => x.GetFlatObjectsListWithInterface<IHasDynamicProperties>());
            using (var repository = _repositoryFactory())
            {
                //Optimize performance and CPU usage
                repository.DisableChangesTracking();

                var objectTypeNames = propOwners.Select(x => x.ObjectType ?? x.GetType().FullName).Distinct().ToArray();
                var objectIds = propOwners.Select(x => x.Id).Distinct().ToArray();

                //Load properties belongs to given objects types
                var dynamicObjectProps = (await repository.GetObjectDynamicPropertiesAsync(objectTypeNames, objectIds))
                                                   .Select(x => x.ToModel(AbstractTypeFactory<DynamicObjectProperty>.TryCreateInstance()))
                                                   .OfType<DynamicObjectProperty>().ToArray();
                foreach (var propOwner in propOwners)
                {
                    var objectType = propOwner.ObjectType ?? propOwner.GetType().FullName;
                    //Filter only properties with belongs to concrete type
                    propOwner.DynamicProperties = dynamicObjectProps.Where(x => x.ObjectType == objectType)
                                                                    .Select(x => x.Clone())
                                                                    .OfType<DynamicObjectProperty>()
                                                                    .ToList();
                    foreach (var prop in propOwner.DynamicProperties)
                    {
                        prop.ObjectId = propOwner.Id;
                        //Leave only self object values 
                        if (prop.Values != null)
                        {
                            prop.Values = prop.Values.Where(x => x.ObjectId == propOwner.Id).ToList();
                        }
                    }
                }
            }
        }


        public virtual async Task SaveDynamicPropertyValuesAsync(IHasDynamicProperties owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            var objectsWithDynamicProperties = owner.GetFlatObjectsListWithInterface<IHasDynamicProperties>().Where(x => !string.IsNullOrEmpty(x.Id) && !x.DynamicProperties.IsNullOrEmpty());
            //Ensure what all properties have proper ObjectId and ObjectType properties set
            foreach (var obj in objectsWithDynamicProperties)
            {
                foreach (var prop in obj.DynamicProperties)
                {
                    prop.ObjectId = obj.Id;
                    prop.ObjectType = obj.ObjectType ?? obj.GetType().FullName;
                }
            }
            var pkMap = new PrimaryKeyResolvingMap();
            using (var repository = _repositoryFactory())
            {
                var objectTypes = objectsWithDynamicProperties.Select(x => x.ObjectType ?? x.GetType().FullName).Distinct().ToArray();
                //Converting all incoming properties to db entity and group property values of all objects use for that   property.objectType and property.name as complex key 
                var modifiedPropertyEntitiesGroup = objectsWithDynamicProperties.SelectMany(x => x.DynamicProperties.Select(dp => AbstractTypeFactory<DynamicPropertyEntity>.TryCreateInstance().FromModel(dp, pkMap)))
                                                                          .GroupBy(x => $"{x.Name}:{x.ObjectType}");
                var originalPropertyEntitites = (await repository.GetObjectDynamicPropertiesAsync(objectTypes, objectsWithDynamicProperties.Select(x => x.Id).Distinct().ToArray())).ToList();
                foreach (var modifiedPropertyEntityGroupItem in modifiedPropertyEntitiesGroup)
                {
                    var modifiedPropertyObjectValues = modifiedPropertyEntityGroupItem.SelectMany(x => x.ObjectValues)
                                                                                    .Where(x => x.GetValue(EnumUtility.SafeParseFlags(x.ValueType, DynamicPropertyValueType.LongText)) != null)
                                                                                    .ToList();
                    //Try to find original property with same complex key
                    var originalEntity = originalPropertyEntitites.FirstOrDefault(x => $"{x.Name}:{x.ObjectType}".EqualsInvariant(modifiedPropertyEntityGroupItem.Key));
                    if (originalEntity != null)
                    {
                        //Update only property values
                        var comparer = AnonymousComparer.Create((DynamicPropertyObjectValueEntity x) => $"{x.ObjectId}:{x.ObjectType}:{x.Locale}:{x.GetValue(EnumUtility.SafeParseFlags(x.ValueType, DynamicPropertyValueType.LongText))}");
                        modifiedPropertyObjectValues.Patch(originalEntity.ObjectValues, comparer, (sourceValue, targetValue) => { });
                    }
                }

                repository.UnitOfWork.Commit();
                pkMap.ResolvePrimaryKeys();
            }
        }

        public virtual async Task DeleteDynamicPropertyValuesAsync(IHasDynamicProperties owner)
        {
            var objectsWithDynamicProperties = owner.GetFlatObjectsListWithInterface<IHasDynamicProperties>();

            using (var repository = _repositoryFactory())
            {
                foreach (var objectHasDynamicProperties in objectsWithDynamicProperties.Where(x => x.Id != null))
                {
                    var typeName = objectHasDynamicProperties.ObjectType ?? objectHasDynamicProperties.GetType().FullName;
                    var values = await repository.DynamicPropertyObjectValues.Where(v => v.ObjectType == typeName && v.ObjectId == objectHasDynamicProperties.Id)
                                                .ToListAsync();

                    foreach (var value in values)
                    {
                        repository.Remove(value);
                    }
                }
                await repository.UnitOfWork.CommitAsync();
            }
        }

        #endregion

    }
}
