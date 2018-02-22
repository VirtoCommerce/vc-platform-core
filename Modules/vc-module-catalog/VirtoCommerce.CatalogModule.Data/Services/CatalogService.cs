using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CatalogModule.Data.Model;
using VirtoCommerce.CatalogModule.Data.Repositories;
using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CatalogModule.Data.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly AbstractValidator<IHasProperties> _hasPropertyValidator;
        private readonly Func<ICatalogRepository> _repositoryFactory;

        public CatalogService(Func<ICatalogRepository> catalogRepositoryFactory, IMemoryCache cacheManager, AbstractValidator<IHasProperties> hasPropertyValidator)
        {
            _repositoryFactory = catalogRepositoryFactory;
            _memoryCache = cacheManager;
            _hasPropertyValidator = hasPropertyValidator;
        }

        #region ICatalogService Members

        public virtual Catalog[] GetAllCatalogs()
        {
            return PreloadCatalogs().Select(x => x.Value.Clone() as Catalog)
                                          .ToArray();
        }

        public virtual Catalog[] GetByIds(string[] catalogIds)
        {
            //Clone required because client code may change resulting objects
            var result = PreloadCatalogs().Join(catalogIds, pair => pair.Key, id => id, (pair, id) => pair.Value)
                                          .Select(x => x.Clone() as Catalog)
                                          .ToArray();
            return result;
        }

        public virtual void SaveChanges(Catalog[] catalogs)
        {
            var pkMap = new PrimaryKeyResolvingMap();

            using (var repository = _repositoryFactory())
            using (var changeTracker = new ObservableChangeTracker())
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
                        modifiedEntity.Patch(originalEntity);
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                    }
                }
                repository.UnitOfWork.Commit();
                pkMap.ResolvePrimaryKeys();
                //Reset cached catalogs and catalogs
                CatalogCacheRegion.ExpireRegion();
            }
        }

        public void Delete(string[] catalogIds)
        {
            using (var repository = _repositoryFactory())
            {
                repository.RemoveCatalogs(catalogIds);
                repository.UnitOfWork.Commit();
                //Reset cached catalogs and catalogs
                CatalogCacheRegion.ExpireRegion();
            }
        }
        #endregion

        protected virtual Dictionary<string, Catalog> PreloadCatalogs()
        {
            var cacheKey = CacheKey.With(GetType(), "AllCatalogs");
            return _memoryCache.GetOrCreateExclusive("AllCatalogs", (cacheEntry) =>
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
                    }
                    if (!catalog.PropertyValues.IsNullOrEmpty())
                    {
                        //Next need set Property in PropertyValues objects
                        foreach (var propValue in catalog.PropertyValues.ToArray())
                        {
                            propValue.Property = catalog.Properties.Where(x => x.Type == PropertyType.Catalog)
                                                                   .FirstOrDefault(x => x.IsSuitableForValue(propValue));
                            //Because multilingual dictionary values for all languages may not stored in db then need to add it in result manually from property dictionary values
                            var localizedDictValues = propValue.TryGetAllLocalizedDictValues();
                            foreach (var localizedDictValue in localizedDictValues)
                            {
                                if (!catalog.PropertyValues.Any(x => x.ValueId == localizedDictValue.ValueId && x.LanguageCode == localizedDictValue.LanguageCode))
                                {
                                    catalog.PropertyValues.Add(localizedDictValue);
                                }
                            }
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
