using System;
using System.Collections.Generic;
using System.Linq;
using CacheManager.Core;
using FluentValidation;
using VirtoCommerce.CatalogModule.Data.Extensions;
using VirtoCommerce.CatalogModule.Data.Model;
using VirtoCommerce.CatalogModule.Data.Repositories;
using VirtoCommerce.CatalogModule.Data.Services.Validation;
using VirtoCommerce.Domain.Catalog.Events;
using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Domain.Commerce.Services;
using VirtoCommerce.Domain.Common.Events;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CatalogModule.Data.Services
{
    public class CategoryServiceImpl : ServiceBase, ICategoryService
    {
        private readonly ICommerceService _commerceService;
        private readonly IOutlineService _outlineService;
        private readonly ICacheManager<object> _cacheManager;
        private readonly AbstractValidator<IHasProperties> _hasPropertyValidator;
        private readonly Func<ICatalogRepository> _repositoryFactory;
        private readonly ICatalogService _catalogService;
        private readonly IEventPublisher _eventPublisher;

        public CategoryServiceImpl(Func<ICatalogRepository> catalogRepositoryFactory, ICommerceService commerceService, IOutlineService outlineService, ICatalogService catalogService, ICacheManager<object> cacheManager, AbstractValidator<IHasProperties> hasPropertyValidator,
                                   IEventPublisher eventPublisher)
        {
            _repositoryFactory = catalogRepositoryFactory;
            _cacheManager = cacheManager;
            _hasPropertyValidator = hasPropertyValidator;
            _commerceService = commerceService;
            _outlineService = outlineService;
            _catalogService = catalogService;
            _eventPublisher = eventPublisher;
        }

        #region ICategoryService Members
        public virtual Category[] GetByIds(string[] categoryIds, CategoryResponseGroup responseGroup, string catalogId = null)
        {
            var result = new List<Category>();
            var preloadedCategoriesMap = PreloadCategories(catalogId);
            foreach (var categoryId in categoryIds.Where(x => x != null))
            {
                Category category;
                if (preloadedCategoriesMap.TryGetValue(categoryId, out category))
                {
                    result.Add(MemberwiseCloneCategory(category));
                }
            }

            //Reduce details according to response group
            foreach (var category in result)
            {
                ReduceDetails(category, responseGroup);
            }

            return result.ToArray();
        }

        public virtual Category GetById(string categoryId, CategoryResponseGroup responseGroup, string catalogId = null)
        {
            return GetByIds(new[] { categoryId }, responseGroup, catalogId).FirstOrDefault();
        }

        public virtual void Create(Category[] categories)
        {
            if (categories == null)
                throw new ArgumentNullException(nameof(categories));

            SaveChanges(categories);
        }


        public virtual Category Create(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            Create(new[] { category });
            return GetById(category.Id, CategoryResponseGroup.Info);
        }

        public virtual void Update(Category[] categories)
        {
            SaveChanges(categories);
        }

        public virtual void Delete(string[] categoryIds)
        {
            var categories = GetByIds(categoryIds, CategoryResponseGroup.Info);
            var changedEntries = categories
                .Select(c => new GenericChangedEntry<Category>(c, EntryState.Deleted))
                .ToList();

            using (var repository = _repositoryFactory())
            {
                _eventPublisher.Publish(new CategoryChangingEvent(changedEntries));

                repository.RemoveCategories(categoryIds);
                CommitChanges(repository);
                //Reset cached categories and catalogs
                ResetCache();

                _eventPublisher.Publish(new CategoryChangedEvent(changedEntries));
            }
        }

        #endregion
        /// <summary>
        /// Reduce category details according to response group
        /// </summary>
        /// <param name="category"></param>
        /// <param name="respGroup"></param>
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
                category.PropertyValues = null;
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

        protected virtual void SaveChanges(Category[] categories)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<Category>>();

            ValidateCategoryProperties(categories);

            using (var repository = _repositoryFactory())
            using (var changeTracker = GetChangeTracker(repository))
            {
                var dbExistCategories = repository.GetCategoriesByIds(categories.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray(), CategoryResponseGroup.Full);
                foreach (var category in categories)
                {
                    var originalEntity = dbExistCategories.FirstOrDefault(x => x.Id == category.Id);
                    var modifiedEntity = AbstractTypeFactory<CategoryEntity>.TryCreateInstance().FromModel(category, pkMap);
                    if (originalEntity != null)
                    {
                        changeTracker.Attach(originalEntity);
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
                _eventPublisher.Publish(new CategoryChangingEvent(changedEntries));

                CommitChanges(repository);
                pkMap.ResolvePrimaryKeys();
                //Reset cached categories and catalogs
                ResetCache();

                _eventPublisher.Publish(new CategoryChangedEvent(changedEntries));
            }
            //Need add seo separately
            _commerceService.UpsertSeoForObjects(categories.OfType<ISeoSupport>().ToArray());
        }

        protected virtual void ResetCache()
        {
            _cacheManager.ClearRegion(CatalogConstants.CacheRegion);
        }

        // TODO: Move to domain
        protected virtual Category MemberwiseCloneCategory(Category category)
        {
            var retVal = AbstractTypeFactory<Category>.TryCreateInstance();

            // Entity
            retVal.Id = category.Id;

            // AuditableEntity
            retVal.CreatedDate = category.CreatedDate;
            retVal.ModifiedDate = category.ModifiedDate;
            retVal.CreatedBy = category.CreatedBy;
            retVal.ModifiedBy = category.ModifiedBy;

            // Category
            retVal.CatalogId = category.CatalogId;
            retVal.Code = category.Code;
            retVal.IsActive = category.IsActive;
            retVal.IsVirtual = category.IsVirtual;
            retVal.Level = category.Level;
            retVal.Name = category.Name;
            retVal.PackageType = category.PackageType;
            retVal.ParentId = category.ParentId;
            retVal.Path = category.Path;
            retVal.Priority = category.Priority;
            retVal.TaxType = category.TaxType;

            // TODO: clone reference objects
            retVal.Children = category.Children;
            retVal.Outlines = category.Outlines;
            retVal.PropertyValues = category.PropertyValues;
            retVal.SeoInfos = category.SeoInfos;
            retVal.Catalog = category.Catalog;
            retVal.Properties = category.Properties;
            retVal.Parents = category.Parents;
            retVal.Links = category.Links;
            retVal.Images = category.Images;

            return retVal;
        }

        protected virtual Dictionary<string, Category> PreloadCategories(string catalogId)
        {
            return _cacheManager.Get($"AllCategories-{catalogId}", CatalogConstants.CacheRegion, () =>
            {
                CategoryEntity[] entities;
                using (var repository = _repositoryFactory())
                {
                    repository.DisableChangesTracking();

                    entities = repository.GetCategoriesByIds(repository.Categories.Select(x => x.Id).ToArray(), CategoryResponseGroup.Full);
                }
                var result = entities.Select(x => x.ToModel(AbstractTypeFactory<Category>.TryCreateInstance())).ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);

                LoadDependencies(result.Values, result);
                ApplyInheritanceRules(result.Values);

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
            var catalogsMap = _catalogService.GetCatalogsList().ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);
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

                if (!category.PropertyValues.IsNullOrEmpty())
                {
                    //Next need set Property in PropertyValues objects
                    foreach (var propValue in category.PropertyValues.ToArray())
                    {
                        propValue.Property = category.Properties.Where(x => x.Type == PropertyType.Category)
                                                                .FirstOrDefault(x => x.IsSuitableForValue(propValue));
                    }
                }
            }
        }

        private void ValidateCategoryProperties(Category[] categories)
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
