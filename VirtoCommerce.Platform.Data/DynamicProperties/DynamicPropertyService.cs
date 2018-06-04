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
        private readonly IMemoryCache _memoryCache;

        public DynamicPropertyService(Func<IPlatformRepository> repositoryFactory, IMemoryCache memoryCache)
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
                var dbExistProperties = await repository.GetDynamicPropertiesByIdsAsync(properties.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var property in properties)
                {
                    var originalEntity = dbExistProperties.FirstOrDefault(x => x.Id == property.Id);
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
                await repository.UnitOfWork.CommitAsync();
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

            var propOwners = owners.SelectMany(x => x.GetFlatObjectsListWithInterface<IHasDynamicProperties>());
            using (var repository = _repositoryFactory())
            {
                //Optimize performance and CPU usage
                repository.DisableChangesTracking();

                var objectTypeNames = propOwners.Select(x => x.ObjectType).Distinct().ToArray();
                var objectIds = propOwners.Select(x => x.Id).Distinct().ToArray();

                //Load properties belongs to given objects types
                var dynamicObjectProps = (await repository.GetObjectDynamicPropertiesAsync(objectTypeNames, objectIds))
                                                   .Select(x => x.ToModel(AbstractTypeFactory<DynamicObjectProperty>.TryCreateInstance()))
                                                   .OfType<DynamicObjectProperty>();
                foreach (var propOwner in propOwners)
                {
                    var objectType = propOwner.ObjectType;
                    //Filter only properties with belongs to concrete type
                    propOwner.DynamicProperties = dynamicObjectProps.Where(x => x.ObjectType == objectType)
                                                                    .Select(x => x.Clone())
                                                                    .OfType<DynamicObjectProperty>()
                                                                    .ToList();
                    foreach (var prop in propOwner.DynamicProperties)
                    {
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
            //Because one DynamicPropertyEntity may update for multiple object in same time
            //need create fresh repository for each object to prevent collisions and overrides property values
            var objectsHaveDynamicProps = owner.GetFlatObjectsListWithInterface<IHasDynamicProperties>();
            foreach (var objectHasDynamicProps in objectsHaveDynamicProps)
            {
                using (var repository = _repositoryFactory())
                {
                    var pkMap = new PrimaryKeyResolvingMap();
                    if (objectHasDynamicProps.Id != null && !objectHasDynamicProps.DynamicProperties.IsNullOrEmpty())
                    {
                        await TryToResolveTransientPropertiesAsync(objectHasDynamicProps.DynamicProperties, objectHasDynamicProps.ObjectType);
                        //Load all object properties with values
                        var existPropertyEntities = await repository.GetObjectDynamicPropertiesAsync(new[] { objectHasDynamicProps.ObjectType }, new[] { objectHasDynamicProps.Id });
                        foreach (var dynamicProp in objectHasDynamicProps.DynamicProperties)
                        {
                            var originalEntity = existPropertyEntities.FirstOrDefault(x => x.Id == dynamicProp.Id);
                            var modifiedEntity = AbstractTypeFactory<DynamicPropertyEntity>.TryCreateInstance().FromModel(dynamicProp, pkMap);
                            if (originalEntity != null)
                            {
                                modifiedEntity.Patch(originalEntity);
                            }
                        }
                        await repository.UnitOfWork.CommitAsync();
                    }
                }
            }
        }

        public virtual async Task DeleteDynamicPropertyValuesAsync(IHasDynamicProperties owner)
        {
            var objectsWithDynamicProperties = owner.GetFlatObjectsListWithInterface<IHasDynamicProperties>();

            using (var repository = _repositoryFactory())
            {
                foreach (var objectHasDynamicProperties in objectsWithDynamicProperties.Where(x => x.Id != null))
                {
                    var values = await repository.DynamicPropertyObjectValues.Where(v => v.ObjectType == objectHasDynamicProperties.ObjectType && v.ObjectId == objectHasDynamicProperties.Id)
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
        protected virtual async Task TryToResolveTransientPropertiesAsync(IEnumerable<DynamicObjectProperty> properties, string objectType)
        {
            //When creating DynamicProperty manually, many properties remain unfilled (except Name, ValueType and ObjectValues).
            //We have to set them with data from the repository.
            var transistentProperties = properties.Where(x => x.Id == null);
            if (transistentProperties.Any())
            {
                using (var repository = _repositoryFactory())
                {
                    var allPropertiesForType = (await repository.DynamicProperties.Where(x => x.ObjectType == objectType).ToArrayAsync())
                                                    .Select(x => x.ToModel(AbstractTypeFactory<DynamicProperty>.TryCreateInstance()));
                    foreach (var transistentPropery in transistentProperties)
                    {
                        var property = allPropertiesForType.FirstOrDefault(x => x.Name.EqualsInvariant(transistentPropery.Name));
                        if (property != null)
                        {
                            transistentPropery.Id = property.Id;
                            transistentPropery.ObjectType = property.ObjectType;
                            transistentPropery.IsArray = property.IsArray;
                            transistentPropery.IsRequired = property.IsRequired;
                            transistentPropery.ValueType = property.ValueType;
                        }
                    }
                }
            }
        }


    }
}
