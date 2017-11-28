using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Model;
using VirtoCommerce.Platform.Data.Repositories;

namespace VirtoCommerce.Platform.Data.DynamicProperties
{
    public class DynamicPropertyService : ServiceBase, IDynamicPropertyService
    {
        private List<string> _availableTypeNames = new List<string>();
        private readonly Func<IPlatformRepository> _repositoryFactory;

        public DynamicPropertyService(Func<IPlatformRepository> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }
      
        #region IDynamicPropertyService Members

        public void RegisterType(string typeName)
        {
            if (!_availableTypeNames.Contains(typeName, StringComparer.OrdinalIgnoreCase))
            {
                _availableTypeNames.Add(typeName);
            }
        }

        public string[] GetAvailableObjectTypeNames()
        {
            return _availableTypeNames.ToArray();
        }

        public string GetObjectTypeName(Type type)
        {
            return type.FullName;
        }

        public DynamicProperty[] GetProperties(string objectType)
        {
            if (objectType == null)
                throw new ArgumentNullException("objectType");

            var result = new List<DynamicProperty>();

            using (var repository = _repositoryFactory())
            {
                var properties = repository.GetDynamicPropertiesForType(objectType);
                result.AddRange(properties.Select(x => x.ToModel(AbstractTypeFactory<DynamicObjectProperty>.TryCreateInstance())));
            }
            return result.ToArray();
        }

        public DynamicProperty[] SaveProperties(DynamicProperty[] properties)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            var pkMap = new PrimaryKeyResolvingMap();
            using (var repository = _repositoryFactory())
            using (var changeTracker = GetChangeTracker(repository))
            {
                var dbExistProperties = repository.GetDynamicPropertiesByIds(properties.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var property in properties)
                {
                    var originalEntity = dbExistProperties.FirstOrDefault(x => x.Id == property.Id);
                    var modifiedEntity = AbstractTypeFactory<DynamicPropertyEntity>.TryCreateInstance().FromModel(property, pkMap);
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
            return properties;
        }

        public void DeleteProperties(string[] propertyIds)
        {
            if (propertyIds == null)
                throw new ArgumentNullException("propertyIds");

            using (var repository = _repositoryFactory())
            {
                var properties = repository.DynamicProperties
                    .Where(p => propertyIds.Contains(p.Id))
                    .ToList();

                foreach (var property in properties)
                {
                    repository.Remove(property);
                }

                CommitChanges(repository);
            }
        }


        public DynamicPropertyDictionaryItem[] GetDictionaryItems(string propertyId)
        {
            if (propertyId == null)
                throw new ArgumentNullException(nameof(propertyId));

            using (var repository = _repositoryFactory())
            {
                var items = repository.GetDynamicPropertyDictionaryItems(propertyId);
                var result = items.OrderBy(x => x.Name).Select(x => x.ToModel(AbstractTypeFactory< DynamicPropertyDictionaryItem>.TryCreateInstance())).ToArray();
                return result;
            }
        }

        public void SaveDictionaryItems(string propertyId, DynamicPropertyDictionaryItem[] items)
        {
            if (propertyId == null)
                throw new ArgumentNullException("propertyId");
            if (items == null)
                throw new ArgumentNullException("items");

            using (var repository = _repositoryFactory())
            using (var changeTracker = GetChangeTracker(repository))
            {
                var dbExistItems = repository.GetDynamicPropertyDictionaryItems(propertyId).ToList();
                foreach (var item in items)
                {
                    var originalEntity = dbExistItems.FirstOrDefault(x => x.Id == item.Id);
                    var modifiedEntity = AbstractTypeFactory<DynamicPropertyDictionaryItemEntity>.TryCreateInstance().FromModel(item);
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
            }          
        }

        public void DeleteDictionaryItems(string[] itemIds)
        {
            if (itemIds == null)
                throw new ArgumentNullException("itemIds");

            using (var repository = _repositoryFactory())
            {
                var items = repository.DynamicPropertyDictionaryItems
                    .Where(v => itemIds.Contains(v.Id))
                    .ToList();

                foreach (var item in items)
                {
                    repository.Remove(item);
                }

                repository.UnitOfWork.Commit();
            }
        }

        public void LoadDynamicPropertyValues(IHasDynamicProperties owner)
        {
            LoadDynamicPropertyValues(new[] { owner });
        }

        public void LoadDynamicPropertyValues(params IHasDynamicProperties[] owners)
        {
            if (owners == null)
            {
                throw new ArgumentNullException(nameof(owners));
            }

            var propOwners = owners.SelectMany(x => x.GetFlatObjectsListWithInterface<IHasDynamicProperties>());
            using (var repository = _repositoryFactory())
            {
                var objectTypeNames = propOwners.Select(x => GetObjectTypeName(x)).Distinct().ToArray();
                var objectIds = propOwners.Select(x => x.Id).Distinct().ToArray();

                //Load properties belongs to given objects types
                var dynamicObjectProps = repository.GetObjectDynamicProperties(objectTypeNames, objectIds)
                                                   .Select(x => x.ToModel(AbstractTypeFactory<DynamicObjectProperty>.TryCreateInstance()))
                                                   .OfType<DynamicObjectProperty>();
                foreach (var propOwner in propOwners)
                {
                    var objectType = GetObjectTypeName(propOwner);
                    //Filter only properties with belongs to concrete type
                    propOwner.DynamicProperties = dynamicObjectProps.Where(x => x.ObjectType == objectType)
                                                                    .Select(x => x.Clone())
                                                                    .OfType<DynamicObjectProperty>()
                                                                    .ToList();
                    foreach(var prop in propOwner.DynamicProperties)
                    {
                        //Leave only self object values 
                        if (prop.Values != null)
                        {
                            prop.Values = prop.Values.Where(x => x.ObjectId == propOwner.Id).ToList();
                        }
                    }
                    propOwner.ObjectType = GetObjectTypeName(propOwner);
                }
            }
        }
     
      
        public void SaveDynamicPropertyValues(IHasDynamicProperties owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }
            //Because one DynamicPropertyEntity may update for multiple object in same time
            //need create fresh repository for each object to prevent collisions and overrides property values
            var objectsHaveDynamicProps = owner.GetFlatObjectsListWithInterface<IHasDynamicProperties>();
            foreach (var objectHasDynamicProps in objectsHaveDynamicProps)
            {
                using (var repository = _repositoryFactory())
                using (var changeTracker = GetChangeTracker(repository))
                {
                    var pkMap = new PrimaryKeyResolvingMap();
                    if (objectHasDynamicProps.Id != null && !objectHasDynamicProps.DynamicProperties.IsNullOrEmpty())
                    {
                        var objectType = GetObjectTypeName(objectHasDynamicProps);

                        TryToResolveTransientProperties(objectHasDynamicProps.DynamicProperties, objectType);
                        //Load all object properties with values
                        var existPropertyEntities = repository.GetObjectDynamicProperties(new[] { objectType }, new[] { objectHasDynamicProps.Id });
                        foreach (var dynamicProp in objectHasDynamicProps.DynamicProperties)
                        {
                            var originalEntity = existPropertyEntities.FirstOrDefault(x => x.Id == dynamicProp.Id);
                            var modifiedEntity = AbstractTypeFactory<DynamicPropertyEntity>.TryCreateInstance().FromModel(dynamicProp, pkMap);
                            if (originalEntity != null)
                            {
                                changeTracker.Attach(originalEntity);
                                modifiedEntity.Patch(originalEntity);
                            }
                        }
                        CommitChanges(repository);

                        repository.UnitOfWork.Commit();
                    }
                }
            }
        }
      
        public void DeleteDynamicPropertyValues(IHasDynamicProperties owner)
        {
            var objectsWithDynamicProperties = owner.GetFlatObjectsListWithInterface<IHasDynamicProperties>();

            using (var repository = _repositoryFactory())
            {
                foreach (var objectWithDynamicProperties in objectsWithDynamicProperties.Where(x => x.Id != null))
                {
                    var objectType = GetObjectTypeName(objectWithDynamicProperties);
                    var objectId = objectWithDynamicProperties.Id;

                    var values = repository.DynamicPropertyObjectValues
                        .Where(v => v.ObjectType == objectType && v.ObjectId == objectId)
                        .ToList();

                    foreach (var value in values)
                    {
                        repository.Remove(value);
                    }
                }
                repository.UnitOfWork.Commit();
            }
        }

        #endregion

        private string GetObjectTypeName(object obj)
        {
            return GetObjectTypeName(obj.GetType());
        }


        private void TryToResolveTransientProperties(IEnumerable<DynamicObjectProperty> properties, string objectType)
        {
            //When creating DynamicProperty manually, many properties remain unfilled (except Name, ValueType and ObjectValues).
            //We have to set them with data from the repository.
            var transistentProperties = properties.Where(x => x.Id == null);
            if (transistentProperties.Any())
            {
                using (var repository = _repositoryFactory())
                {
                    var allPropertiesForType = repository.GetDynamicPropertiesForType(objectType).Select(x => x.ToModel(AbstractTypeFactory<DynamicProperty>.TryCreateInstance()));
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
