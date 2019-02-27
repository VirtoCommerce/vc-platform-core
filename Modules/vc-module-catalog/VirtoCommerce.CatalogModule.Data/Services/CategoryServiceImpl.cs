using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CatalogModule.Core.Events;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CatalogModule.Data.Caching;
using VirtoCommerce.CatalogModule.Data.Model;
using VirtoCommerce.CatalogModule.Data.Repositories;
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

        public CategoryServiceImpl(Func<ICatalogRepository> catalogRepositoryFactory,
                                   IEventPublisher eventPublisher, IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = catalogRepositoryFactory;
            _eventPublisher = eventPublisher;
            _platformMemoryCache = platformMemoryCache;
        }

        #region ICategoryService Members
        public virtual Task<Category[]> GetByIdsAsync(string[] categoryIds, CategoryResponseGroup responseGroup, string catalogId = null)
        {
            var cacheKey = CacheKey.With(GetType(), "GetByIdsAsync", string.Join(",", categoryIds));
            return _platformMemoryCache.GetOrCreateExclusive(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CategoryCacheRegion.CreateChangeToken());
                CategoryEntity[] entities;
                using (var repository = _repositoryFactory())
                {
                    repository.DisableChangesTracking();

                    entities = await repository.GetCategoriesByIdsAsync(repository.Categories.Select(x => x.Id).ToArray(), responseGroup);
                }
                return entities.Select(x => x.ToModel(AbstractTypeFactory<Category>.TryCreateInstance())).ToArray();
            });
        }

        public virtual async Task SaveChangesAsync(Category[] categories)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<Category>>();

            ValidateCategoryProperties(categories);

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

                //TODO Outline to eventhandler
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


        //TODO
        private void ValidateCategoryProperties(Category[] categories)
        {
            //if (categories == null)
            //{
            //    throw new ArgumentNullException(nameof(categories));
            //}
            ////Validate categories 
            //var validator = new CategoryValidator();
            //foreach (var category in categories)
            //{
            //    validator.ValidateAndThrow(category);
            //}

            //var groups = categories.GroupBy(x => x.CatalogId);
            //foreach (var group in groups)
            //{
            //    LoadDependencies(group, PreloadCategories(group.Key));
            //    ApplyInheritanceRules(group);

            //    foreach (var category in group)
            //    {
            //        var validatioResult = _hasPropertyValidator.Validate(category);
            //        if (!validatioResult.IsValid)
            //        {
            //            throw new Exception($"Category properties has validation error: {string.Join(Environment.NewLine, validatioResult.Errors.Select(x => x.ToString()))}");
            //        }
            //    }
            //}
        }
    }
}
