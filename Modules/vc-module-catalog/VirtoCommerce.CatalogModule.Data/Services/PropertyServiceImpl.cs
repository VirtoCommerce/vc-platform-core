using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Events;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CatalogModule.Data.Model;
using VirtoCommerce.CatalogModule.Data.Repositories;
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

        public PropertyServiceImpl(Func<ICatalogRepository> repositoryFactory, IEventPublisher eventPublisher)
        {
            _repositoryFactory = repositoryFactory;
            _eventPublisher = eventPublisher;
        }

        #region IPropertyService Members


        public virtual async Task<Property[]> GetByIdsAsync(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                repository.DisableChangesTracking();

                var entities = await repository.GetPropertiesByIdsAsync(ids);
                var properties = entities.Select(p => p.ToModel(AbstractTypeFactory<Property>.TryCreateInstance())).ToArray();

                ApplyInheritanceRules(properties);

                return properties;
            }
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

                //TODO
                //Reset cached categories and catalogs
                //ResetCache();

                await _eventPublisher.Publish(new PropertyChangedEvent(changedEntries));
            }
        }

        public async Task DeleteAsync(string[] propertyIds)
        {
            var changedEntries = (await GetByIdsAsync(propertyIds))
                .Select(p => new GenericChangedEntry<Property>(p, EntryState.Deleted))
                .ToList();

            using (var repository = _repositoryFactory())
            {
                var entities = await repository.GetPropertiesByIdsAsync(propertyIds);

                await _eventPublisher.Publish(new PropertyChangingEvent(changedEntries));

                foreach (var entity in entities)
                {
                    repository.Remove(entity);
                }

                await repository.UnitOfWork.CommitAsync();

                //TODO
                //Reset cached categories and catalogs
                //ResetCache();

                await _eventPublisher.Publish(new PropertyChangedEvent(changedEntries));
            }
        }

        #endregion

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
