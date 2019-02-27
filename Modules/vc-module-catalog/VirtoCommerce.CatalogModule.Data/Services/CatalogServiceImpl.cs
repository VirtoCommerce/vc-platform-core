using System;
using System.Collections.Generic;
using System.Linq;
using CacheManager.Core;
using FluentValidation;
using VirtoCommerce.CatalogModule.Data.Model;
using VirtoCommerce.CatalogModule.Data.Repositories;
using VirtoCommerce.Domain.Catalog.Events;
using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Domain.Common.Events;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CatalogModule.Data.Services
{
    public class CatalogServiceImpl : ServiceBase, ICatalogService
    {
        private readonly ICacheManager<object> _cacheManager;
        private readonly AbstractValidator<IHasProperties> _hasPropertyValidator;
        private readonly Func<ICatalogRepository> _repositoryFactory;
        private readonly IEventPublisher _eventPublisher;

        public CatalogServiceImpl(Func<ICatalogRepository> catalogRepositoryFactory, ICacheManager<object> cacheManager, AbstractValidator<IHasProperties> hasPropertyValidator,
                                  IEventPublisher eventPublisher)
        {
            _repositoryFactory = catalogRepositoryFactory;
            _cacheManager = cacheManager;
            _hasPropertyValidator = hasPropertyValidator;
            _eventPublisher = eventPublisher;
        }

        #region ICatalogService Members

        public Catalog GetById(string catalogId)
        {
            Catalog result;
            if (PreloadCatalogs().TryGetValue(catalogId, out result))
            {
                //Clone required because client code may change resulting objects
                result = MemberwiseCloneCatalog(result);
            }
            return result;
        }

        public Catalog Create(Catalog catalog)
        {
            SaveChanges(new[] { catalog });
            var result = GetById(catalog.Id);
            return result;
        }

        public void Update(Catalog[] catalogs)
        {
            SaveChanges(catalogs);
        }

        public void Delete(string[] catalogIds)
        {
            var changedEntries = GetByIds(catalogIds)
                .Select(c => new GenericChangedEntry<Catalog>(c, EntryState.Deleted))
                .ToList();

            using (var repository = _repositoryFactory())
            {
                _eventPublisher.Publish(new CatalogChangingEvent(changedEntries));

                repository.RemoveCatalogs(catalogIds);
                CommitChanges(repository);
                //Reset cached catalogs and catalogs
                ResetCache();

                _eventPublisher.Publish(new CatalogChangedEvent(changedEntries));
            }
        }

        protected virtual Catalog[] GetByIds(string[] catalogIds)
        {
            return PreloadCatalogs()
                .Values
                .Where(c => catalogIds.Contains(c.Id))
                .Select(MemberwiseCloneCatalog)
                .ToArray();
        }

        public IEnumerable<Catalog> GetCatalogsList()
        {
            //Clone required because client code may change resulting objects
            return PreloadCatalogs().Values.Select(MemberwiseCloneCatalog).OrderBy(x => x.Name);
        }

        #endregion

        protected virtual void SaveChanges(Catalog[] catalogs)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<Catalog>>();

            using (var repository = _repositoryFactory())
            using (var changeTracker = GetChangeTracker(repository))
            {
                ValidateCatalogProperties(catalogs);
                var dbExistEntities = repository.GetCatalogsByIds(catalogs.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var catalog in catalogs)
                {
                    var originalEntity = dbExistEntities.FirstOrDefault(x => x.Id == catalog.Id);
                    var modifiedEntity = AbstractTypeFactory<CatalogEntity>.TryCreateInstance().FromModel(catalog, pkMap);
                    if (originalEntity != null)
                    {
                        changeTracker.Attach(originalEntity);

                        changedEntries.Add(new GenericChangedEntry<Catalog>(catalog, originalEntity.ToModel(AbstractTypeFactory<Catalog>.TryCreateInstance()), EntryState.Modified));

                        modifiedEntity.Patch(originalEntity);
                    }
                    else
                    {
                        repository.Add(modifiedEntity);

                        changedEntries.Add(new GenericChangedEntry<Catalog>(catalog, EntryState.Added));
                    }
                }

                _eventPublisher.Publish(new CatalogChangingEvent(changedEntries));

                CommitChanges(repository);
                pkMap.ResolvePrimaryKeys();
                //Reset cached catalogs and catalogs
                ResetCache();

                _eventPublisher.Publish(new CatalogChangedEvent(changedEntries));
            }
        }

        protected virtual void ResetCache()
        {
            _cacheManager.ClearRegion(CatalogConstants.CacheRegion);
        }


        protected virtual Catalog MemberwiseCloneCatalog(Catalog catalog)
        {
            var retVal = AbstractTypeFactory<Catalog>.TryCreateInstance();

            retVal.Id = catalog.Id;
            retVal.IsVirtual = catalog.IsVirtual;
            retVal.Name = catalog.Name;

            // TODO: clone reference objects
            retVal.Languages = catalog.Languages;
            retVal.Properties = catalog.Properties;
            retVal.PropertyValues = catalog.PropertyValues;

            return retVal;
        }

        protected virtual Dictionary<string, Catalog> PreloadCatalogs()
        {
            return _cacheManager.Get("AllCatalogs", CatalogConstants.CacheRegion, () =>
            {
                CatalogEntity[] entities;
                using (var repository = _repositoryFactory())
                {
                    //Optimize performance and CPU usage
                    repository.DisableChangesTracking();

                    entities = repository.GetCatalogsByIds(repository.Catalogs.Select(x => x.Id).ToArray());
                }

                var result = entities.Select(x => x.ToModel(AbstractTypeFactory<Catalog>.TryCreateInstance())).ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);

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
                Catalog preloadedCatalog;
                if (preloadedCatalogsMap.TryGetValue(catalog.Id, out preloadedCatalog))
                {
                    catalog.Properties = preloadedCatalog.Properties;
                    foreach (var property in catalog.Properties)
                    {
                        property.Catalog = preloadedCatalogsMap[property.CatalogId];
                    }
                    if (!catalog.PropertyValues.IsNullOrEmpty())
                    {
                        //Next need set Property in PropertyValues objects
                        foreach (var propValue in catalog.PropertyValues.ToArray())
                        {
                            propValue.Property = catalog.Properties.Where(x => x.Type == PropertyType.Catalog)
                                                                   .FirstOrDefault(x => x.IsSuitableForValue(propValue));
                        }
                    }
                }
            }
        }

        private void ValidateCatalogProperties(Catalog[] catalogs)
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
