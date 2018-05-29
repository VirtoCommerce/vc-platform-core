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
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Domain.Commerce.Services;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CatalogModule.Data.Services
{
    public class CategoryService : ServiceBase, ICategoryService
    {
        private readonly ICommerceService _commerceService;
        private readonly IOutlineService _outlineService;
        private readonly IMemoryCache _memoryCache;
        private readonly AbstractValidator<IHasProperties> _hasPropertyValidator;
        private readonly Func<ICatalogRepository> _repositoryFactory;
        private readonly ICatalogService _catalogService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IBlobUrlResolver _blobUrlResolver;

        public CategoryService(Func<ICatalogRepository> catalogRepositoryFactory, ICommerceService commerceService, IOutlineService outlineService, ICatalogService catalogService, IMemoryCache memoryCache,
                                   AbstractValidator<IHasProperties> hasPropertyValidator, IEventPublisher eventPublisher, IBlobUrlResolver blobUrlResolver)
        {
            _repositoryFactory = catalogRepositoryFactory;
            _memoryCache = memoryCache;
            _hasPropertyValidator = hasPropertyValidator;
            _commerceService = commerceService;
            _outlineService = outlineService;
            _catalogService = catalogService;
            _eventPublisher = eventPublisher;
            _blobUrlResolver = blobUrlResolver;
        }

        #region ICategoryService Members
        public virtual IEnumerable<Category> GetByIds(IEnumerable<string> categoryIds, string responseGroup = null, string catalogId = null)
        {
            var result = new List<Category>();
            var preloadedCategoriesMap = PreloadCategories(catalogId);
            foreach (var categoryId in categoryIds.Where(x => x != null))
            {
                if (preloadedCategoriesMap.TryGetValue(categoryId, out var category))
                {
                    result.Add(category.Clone() as Category);
                }
            }

            //Reduce details according to response group
            foreach (var category in result)
            {
                category.ReduceDetails(responseGroup);
            }

            return result.ToArray();
        }


        public virtual void SaveChanges(IEnumerable<Category> categories)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<Category>>();

            ValidateCategoryProperties(categories);

            using (var repository = _repositoryFactory())
            {
                var dbExistCategories = repository.GetCategoriesByIds(categories.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var category in categories)
                {
                    var originalEntity = dbExistCategories.FirstOrDefault(x => x.Id == category.Id);
                    var modifiedEntity = AbstractTypeFactory<CategoryEntity>.TryCreateInstance().FromModel(category, pkMap);
                    if (originalEntity != null)
                    {
                        changedEntries.Add(new GenericChangedEntry<Category>(category, originalEntity.ToModel(AbstractTypeFactory<Category>.TryCreateInstance()), EntryState.Modified));
                        modifiedEntity.Patch(originalEntity);
                        //Force set ModifiedDate property to mark a product changed. Special for  partial update cases when product table not have changes
                        originalEntity.ModifiedDate = DateTime.UtcNow;
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                        changedEntries.Add(new GenericChangedEntry<Category>(category, EntryState.Added));
                    }
                }

                //Raise domain events
                _eventPublisher.Publish(new CategoryChangingEvent(changedEntries));
                //Save changes in database
                repository.UnitOfWork.Commit();
                pkMap.ResolvePrimaryKeys();
                _eventPublisher.Publish(new CategoryChangedEvent(changedEntries));
                //Reset catalog cache
                CatalogCacheRegion.ExpireRegion();
            }
            //Need add seo separately
            _commerceService.UpsertSeoForObjects(categories.OfType<ISeoSupport>().ToArray());
        }


        public virtual void Delete(IEnumerable<string> categoryIds)
        {
            var categories = GetByIds(categoryIds, CategoryResponseGroup.Info.ToString());
            //Raise domain events before deletion
            var changedEntries = categories.Select(x => new GenericChangedEntry<Category>(x, EntryState.Deleted));
            _eventPublisher.Publish(new CategoryChangingEvent(changedEntries));
            using (var repository = _repositoryFactory())
            {
                //TODO: raise events on categories deletion
                repository.RemoveCategories(categoryIds.ToArray());
                CommitChanges(repository);
                //Reset catalog cache
                CatalogCacheRegion.ExpireRegion();
            }
            _eventPublisher.Publish(new CategoryChangedEvent(changedEntries));
        }

        #endregion


        protected virtual Dictionary<string, Category> PreloadCategories(string catalogId)
        {
            var cacheKey = CacheKey.With(GetType(), "PreloadCategories", catalogId);
            return _memoryCache.GetOrCreateExclusive(cacheKey, (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CatalogCacheRegion.CreateChangeToken());
                CategoryEntity[] entities;
                using (var repository = _repositoryFactory())
                {
                    repository.DisableChangesTracking();

                    entities = repository.GetCategoriesByIds(repository.Categories.Select(x => x.Id).ToArray());
                }
                var result = entities.Select(x => x.ToModel(AbstractTypeFactory<Category>.TryCreateInstance())).ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);

                LoadDependencies(result.Values, result);
                ApplyInheritanceRules(result.Values.OrderBy(x => x.Level));

                // Fill outlines for categories            
                _outlineService.FillOutlinesForObjects(result.Values, catalogId);

                var objectsWithSeo = new List<ISeoSupport>(result.Values);
                var outlineItems = result.Values.Where(c => c.Outlines != null).SelectMany(c => c.Outlines.SelectMany(o => o.Items));
                objectsWithSeo.AddRange(outlineItems);
                _commerceService.LoadSeoForObjects(objectsWithSeo.ToArray());
                return result;
            });
        }

        protected virtual void LoadDependencies(IEnumerable<Category> categories, Dictionary<string, Category> preloadedCategoriesMap)
        {
            var catalogsMap = _catalogService.GetAllCatalogs().ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);
            foreach (var category in categories)
            {
                category.Catalog = catalogsMap.GetValueOrThrow(category.CatalogId, $"catalog with key {category.CatalogId} doesn't exist");
                category.IsVirtual = category.Catalog.IsVirtual;
                category.Parents = Array.Empty<Category>();
                //Load all parent categories
                if (category.ParentId != null)
                {
                    category.Parents = TreeExtension.GetAncestors(category, x => x.ParentId != null && preloadedCategoriesMap.ContainsKey(x.ParentId) ? preloadedCategoriesMap[x.ParentId] : null)
                                                    .Reverse()
                                                    .ToArray();
                    category.Parent = category.Parents.Last();
                }
                category.Level = category.Parents?.Count() ?? 0;

                if (!category.Links.IsNullOrEmpty())
                {
                    foreach (var link in category.Links)
                    {
                        link.Catalog = catalogsMap.GetValueOrThrow(link.CatalogId, $"link catalog with key {link.CatalogId} doesn't exist");
                        if (link.CategoryId != null)
                        {
                            if (preloadedCategoriesMap.ContainsKey(link.CategoryId))
                            {
                                link.Category = preloadedCategoriesMap[link.CategoryId];
                            }
                        }
                    }
                }

                if (!category.Properties.IsNullOrEmpty())
                {
                    foreach (var property in category.Properties)
                    {
                        property.Catalog = catalogsMap.GetValueOrThrow(property.CatalogId, $"property catalog with key {property.CatalogId} doesn't exist");
                        if (property.CategoryId != null)
                        {
                            if (preloadedCategoriesMap.ContainsKey(property.CategoryId))
                            {
                                property.Category = preloadedCategoriesMap[property.CategoryId];
                            }
                        }
                    }
                }
                //Resolve relative urls for category assets
                if (category.AllAssets != null)
                {
                    foreach (var asset in category.AllAssets)
                    {
                        asset.Url = _blobUrlResolver.GetAbsoluteUrl(asset.RelativeUrl);
                    }
                }
            }

        }

        protected virtual void ApplyInheritanceRules(IEnumerable<Category> categories)
        {
            foreach (var category in categories)
            {
                category.TryInheritFrom(category.Parent ?? (IEntity)category.Catalog);
            }
        }

        private void ValidateCategoryProperties(IEnumerable<Category> categories)
        {
            if (categories == null)
            {
                throw new ArgumentNullException(nameof(categories));
            }
            //Validate categories 
            var validator = new CategoryValidator();
            foreach (var category in categories)
            {
                validator.ValidateAndThrow(category);
            }

            var groups = categories.GroupBy(x => x.CatalogId);
            foreach (var group in groups)
            {
                LoadDependencies(group, PreloadCategories(group.Key));
                ApplyInheritanceRules(group);

                foreach (var category in group)
                {
                    var validatioResult = _hasPropertyValidator.Validate(category);
                    if (!validatioResult.IsValid)
                    {
                        throw new Exception($"Category properties has validation error: {string.Join(Environment.NewLine, validatioResult.Errors.Select(x => x.ToString()))}");
                    }
                }
            }
        }
    }
}
