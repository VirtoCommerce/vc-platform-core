using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CatalogModule.Core.Events;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CatalogModule.Data.Caching;
using VirtoCommerce.CatalogModule.Data.Model;
using VirtoCommerce.CatalogModule.Data.Repositories;
using VirtoCommerce.CatalogModule.Data.Validation;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CatalogModule.Data.Services
{
    public class CategoryServiceImpl : ICategoryService
    {
        private readonly IPlatformMemoryCache _platformMemoryCache;
        private readonly Func<ICatalogRepository> _repositoryFactory;
        private readonly IEventPublisher _eventPublisher;
        private readonly AbstractValidator<IHasProperties> _hasPropertyValidator;
        private readonly ICatalogService _catalogService;
        private readonly IOutlineService _outlineService;
        private readonly ISeoService _seoService;

        public CategoryServiceImpl(Func<ICatalogRepository> catalogRepositoryFactory,
                                   IEventPublisher eventPublisher, IPlatformMemoryCache platformMemoryCache, AbstractValidator<IHasProperties> hasPropertyValidator, ICatalogService catalogService, IOutlineService outlineService, ISeoService seoService)
        {
            _repositoryFactory = catalogRepositoryFactory;
            _eventPublisher = eventPublisher;
            _platformMemoryCache = platformMemoryCache;
            _hasPropertyValidator = hasPropertyValidator;
            _catalogService = catalogService;
            _outlineService = outlineService;
            _seoService = seoService;
        }

        #region ICategoryService Members
        public virtual async Task<Category[]> GetByIdsAsync(string[] categoryIds, CategoryResponseGroup responseGroup, string catalogId = null)
        {
            var result = new List<Category>();
            var preloadedCategoriesMap = await PreloadCategories(catalogId);
            foreach (var categoryId in categoryIds.Where(x => x != null))
            {
                Category category;
                if (preloadedCategoriesMap.TryGetValue(categoryId, out category))
                {
                    result.Add(category.MemberwiseCloneCategory());
                }
            }

            //Reduce details according to response group
            foreach (var category in result)
            {
                ReduceDetails(category, responseGroup);
            }

            return result.ToArray();
        }

        public virtual async Task SaveChangesAsync(Category[] categories)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<Category>>();

            await ValidateCategoryPropertiesAsync(categories);

            using (var repository = _repositoryFactory())
            {
                var dbExistCategories = await repository.GetCategoriesByIdsAsync(categories.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray(), CategoryResponseGroup.Full);
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

                await _eventPublisher.Publish(new CategoryChangingEvent(changedEntries));

                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();
                CategoryCacheRegion.ExpireRegion();

                await _eventPublisher.Publish(new CategoryChangedEvent(changedEntries));
            }
        }

        public virtual async Task DeleteAsync(string[] categoryIds)
        {
            var categories = await GetByIdsAsync(categoryIds, CategoryResponseGroup.Info);
            var changedEntries = categories
                .Select(c => new GenericChangedEntry<Category>(c, EntryState.Deleted))
                .ToList();

            using (var repository = _repositoryFactory())
            {
                await _eventPublisher.Publish(new CategoryChangingEvent(changedEntries));
                await repository.RemoveCategoriesAsync(categoryIds);
                await repository.UnitOfWork.CommitAsync();

                CategoryCacheRegion.ExpireRegion();
                await _eventPublisher.Publish(new CategoryChangedEvent(changedEntries));
            }
        }

        #endregion

        /// <summary>
        /// Reduce category details according to response group
        /// </summary>
        /// <param name="category"></param>
        /// <param name="responseGroup"></param>
        protected virtual void ReduceDetails(Category category, CategoryResponseGroup responseGroup)
        {
            if (!responseGroup.HasFlag(CategoryResponseGroup.WithImages))
            {
                category.Images = null;
            }
            if (!responseGroup.HasFlag(CategoryResponseGroup.WithLinks))
            {
                category.Links = null;
            }
            if (!responseGroup.HasFlag(CategoryResponseGroup.WithParents))
            {
                category.Parents = null;
            }
            if (!responseGroup.HasFlag(CategoryResponseGroup.WithProperties))
            {
                category.Properties = null;
            }
            if (!responseGroup.HasFlag(CategoryResponseGroup.WithOutlines))
            {
                category.Outlines = null;
            }
            if (!responseGroup.HasFlag(CategoryResponseGroup.WithSeo))
            {
                category.SeoInfos = null;
            }
        }

        protected virtual Task<Dictionary<string, Category>> PreloadCategories(string catalogId)
        {
            var cacheKey = CacheKey.With(GetType(), "PreloadCategories", catalogId);
            return _platformMemoryCache.GetOrCreateExclusive(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CategoryCacheRegion.CreateChangeToken());
                CategoryEntity[] entities;
                using (var repository = _repositoryFactory())
                {
                    repository.DisableChangesTracking();

                    entities = await repository.GetCategoriesByIdsAsync(repository.Categories.Select(x => x.Id).ToArray(), CategoryResponseGroup.Full);
                }
                var result = entities.Select(x => x.ToModel(AbstractTypeFactory<Category>.TryCreateInstance())).ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);

                await LoadDependenciesAsync(result.Values, result);
                ApplyInheritanceRules(result.Values);

                // Fill outlines for categories            
                _outlineService.FillOutlinesForObjects(result.Values, catalogId);

                var objectsWithSeo = new List<ISeoSupport>(result.Values);
                var outlineItems = result.Values.Where(c => c.Outlines != null).SelectMany(c => c.Outlines.SelectMany(o => o.Items));
                objectsWithSeo.AddRange(outlineItems);
                await _seoService.LoadSeoForObjectsAsync(objectsWithSeo.ToArray());

                return result;
            });
        }

        protected virtual async Task LoadDependenciesAsync(IEnumerable<Category> categories, Dictionary<string, Category> preloadedCategoriesMap)
        {
            var catalogsMap = (await _catalogService.GetCatalogsListAsync()).ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);
            foreach (var category in categories)
            {
                category.Catalog = catalogsMap.GetValueOrThrow(category.CatalogId, $"catalog with key {category.CatalogId} not exist");
                category.IsVirtual = category.Catalog.IsVirtual;
                category.Parents = Array.Empty<Category>();
                //Load all parent categories
                if (category.ParentId != null)
                {
                    category.Parents = TreeExtension.GetAncestors(category, x => x.ParentId != null && preloadedCategoriesMap.ContainsKey(x.ParentId) ? preloadedCategoriesMap[x.ParentId] : null)
                                                    .Reverse()
                                                    .ToArray();
                }
                category.Level = category.Parents.Length;

                if (!category.Links.IsNullOrEmpty())
                {
                    foreach (var link in category.Links)
                    {
                        link.Catalog = catalogsMap.GetValueOrThrow(link.CatalogId, $"link catalog with key {link.CatalogId} not exist");
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
                        property.Catalog = catalogsMap.GetValueOrThrow(property.CatalogId, $"property catalog with key {property.CatalogId} not exist");
                        if (property.CategoryId != null)
                        {
                            if (preloadedCategoriesMap.ContainsKey(property.CategoryId))
                            {
                                property.Category = preloadedCategoriesMap[property.CategoryId];
                            }
                        }
                    }
                }
            }

        }

        protected virtual void ApplyInheritanceRules(IEnumerable<Category> categories)
        {
            foreach (var category in categories)
            {
                //Try to inherit taxType from parent category
                if (category.TaxType == null && !category.Parents.IsNullOrEmpty())
                {
                    category.TaxType = category.Parents.Select(x => x.TaxType).FirstOrDefault(x => x != null);
                }
                //Inherit properties
                var properties = category.Catalog.Properties.ToList();
                //For parents categories                       
                properties.AddRange(category.Parents.SelectMany(x => x.Properties));
                if (category.Properties != null)
                {
                    // Self properties
                    properties.AddRange(category.Properties);
                }

                //property override - need leave only property has a min distance to target category 
                //Algorithm based on index property in resulting list (property with min index will more closed to category)
                category.Properties = properties.Select((x, index) => new { Property = x, Index = index })
                                           .GroupBy(x => $"{x.Property.Name.ToLowerInvariant()}:{x.Property.Type}")
                                           .Select(x => x.OrderBy(y => y.Index).First().Property)
                                           .OrderBy(x => x.Name)
                                           .ToList();

                if (!category.Properties.IsNullOrEmpty())
                {
                    foreach (var property in category.Properties)
                    {
                        //Next need set Property in PropertyValues objects
                        foreach (var propValue in property.Values.ToArray())
                        {
                            propValue.Property = category.Properties.Where(x => x.Type == PropertyType.Category)
                                .FirstOrDefault(x => x.IsSuitableForValue(propValue));
                        }
                    }

                }
            }
        }


        private async Task ValidateCategoryPropertiesAsync(Category[] categories)
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
                await LoadDependenciesAsync(group, await PreloadCategories(group.Key));
                ApplyInheritanceRules(group);

                foreach (var category in group)
                {
                    var validatioResult = await _hasPropertyValidator.ValidateAsync(category);
                    if (!validatioResult.IsValid)
                    {
                        throw new Exception($"Category properties has validation error: {string.Join(Environment.NewLine, validatioResult.Errors.Select(x => x.ToString()))}");
                    }
                }
            }
        }
    }
}
