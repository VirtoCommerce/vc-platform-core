using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using VirtoCommerce.CatalogModule.Core.Events;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CatalogModule.Data.Model;
using VirtoCommerce.CatalogModule.Data.Repositories;
using VirtoCommerce.CatalogModule.Data.Validation;
using VirtoCommerce.CoreModule.Core.Commerce.Services;
using VirtoCommerce.CoreModule.Core.Model;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CatalogModule.Data.Services
{
    public class ItemService : IItemService
    {
        private readonly ICategoryService _categoryService;
        private readonly ICatalogService _catalogService;
        private readonly ICommerceService _commerceService;
        private readonly IOutlineService _outlineService;
        private readonly Func<ICatalogRepository> _repositoryFactory;
        private readonly AbstractValidator<IHasProperties> _hasPropertyValidator;
        private readonly IEventPublisher _eventPublisher;
        private readonly IBlobUrlResolver _blobUrlResolver;
        private readonly ISkuGenerator _skuGenerator;
        public ItemService(Func<ICatalogRepository> catalogRepositoryFactory, ICommerceService commerceService, IOutlineService outlineService, ICatalogService catalogService,
                               ICategoryService categoryService, AbstractValidator<IHasProperties> hasPropertyValidator, IEventPublisher eventPublisher, IBlobUrlResolver blobUrlResolver, ISkuGenerator skuGenerator)
        {
            _catalogService = catalogService;
            _categoryService = categoryService;
            _commerceService = commerceService;
            _outlineService = outlineService;
            _repositoryFactory = catalogRepositoryFactory;
            _hasPropertyValidator = hasPropertyValidator;
            _eventPublisher = eventPublisher;
            _blobUrlResolver = blobUrlResolver;
            _skuGenerator = skuGenerator;
        }

        #region IItemService Members

        public virtual IEnumerable<CatalogProduct> GetByIds(IEnumerable<string> itemIds, string respGroup = null, string catalogId = null)
        {
            CatalogProduct[] result;
            var itemRespGroup = EnumUtility.SafeParse(respGroup, ItemResponseGroup.ItemLarge);
            using (var repository = _repositoryFactory())
            {
                //Optimize performance and CPU usage
                repository.DisableChangesTracking();

                result = repository.GetItemByIds(itemIds.ToArray(), itemRespGroup)
                                   .Select(x => x.ToModel(AbstractTypeFactory<CatalogProduct>.TryCreateInstance()))
                                   .ToArray();
            }

            InnerLoadDependencies(result);
            ApplyInheritanceRules(result);

            var productsWithVariationsList = result.Concat(result.Where(p => p.Variations != null)
                                       .SelectMany(p => p.Variations));
            // Fill outlines for products and variations
            if (itemRespGroup.HasFlag(ItemResponseGroup.Outlines))
            {
                _outlineService.FillOutlinesForObjects(productsWithVariationsList, catalogId);
            }
            // Fill SEO info for products, variations and outline items
            if ((itemRespGroup & ItemResponseGroup.Seo) == ItemResponseGroup.Seo)
            {
                var objectsWithSeo = productsWithVariationsList.OfType<ISeoSupport>().ToList();
                //Load SEO information for all Outline.Items
                var outlineItems = productsWithVariationsList.Where(p => p.Outlines != null)
                                         .SelectMany(p => p.Outlines.SelectMany(o => o.Items));
                objectsWithSeo.AddRange(outlineItems);
                //TODO: convert to async
                _commerceService.LoadSeoForObjectsAsync(objectsWithSeo.ToArray());
            }

            //Reduce details according to response group
            foreach (var product in productsWithVariationsList)
            {
                product.ReduceDetails(respGroup);
            }

            return result;
        }

        public virtual void SaveChanges(IEnumerable<CatalogProduct> products)
        {
            InnerSaveChanges(products);
        }

        public virtual void Delete(IEnumerable<string> itemIds)
        {
            var products = GetByIds(itemIds, ItemResponseGroup.ItemInfo.ToString());
            //Raise domain events before deletion
            var changedEntries = products.Select(x => new GenericChangedEntry<CatalogProduct>(x, EntryState.Deleted));

            _eventPublisher.Publish(new ProductChangingEvent(changedEntries));

            using (var repository = _repositoryFactory())
            {
                repository.RemoveItems(itemIds.ToArray());
                repository.UnitOfWork.Commit();
            }

            _eventPublisher.Publish(new ProductChangedEvent(changedEntries));
        }

        #endregion


        protected virtual void InnerSaveChanges(IEnumerable<CatalogProduct> products, bool disableValidation = false)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<CatalogProduct>>();

            ValidateProducts(products);

            using (var repository = _repositoryFactory())
            {
                var dbExistProducts = repository.GetItemByIds(products.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray(), ItemResponseGroup.ItemLarge);
                foreach (var product in products)
                {
                    var modifiedEntity = AbstractTypeFactory<ItemEntity>.TryCreateInstance().FromModel(product, pkMap);
                    var originalEntity = dbExistProducts.FirstOrDefault(x => x.Id == product.Id);

                    if (originalEntity != null)
                    {
                        changedEntries.Add(new GenericChangedEntry<CatalogProduct>(product, originalEntity.ToModel(AbstractTypeFactory<CatalogProduct>.TryCreateInstance()), EntryState.Modified));
                        modifiedEntity.Patch(originalEntity);
                        //Force set ModifiedDate property to mark a product changed. Special for  partial update cases when product table not have changes
                        originalEntity.ModifiedDate = DateTime.UtcNow;
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                        changedEntries.Add(new GenericChangedEntry<CatalogProduct>(product, EntryState.Added));
                    }
                }

                //Raise domain events
                _eventPublisher.Publish(new ProductChangingEvent(changedEntries));
                //Save changes in database
                repository.UnitOfWork.Commit();
                pkMap.ResolvePrimaryKeys();
                _eventPublisher.Publish(new ProductChangedEvent(changedEntries));
            }

            //Update SEO 
            var productsWithVariations = products.Concat(products.Where(x => x.Variations != null).SelectMany(x => x.Variations)).OfType<ISeoSupport>().ToArray();
            _commerceService.UpsertSeoForObjectsAsync(productsWithVariations);
        }

        public virtual void LoadDependencies(IEnumerable<CatalogProduct> products)
        {
            InnerLoadDependencies(products);
        }

        protected virtual void InnerLoadDependencies(IEnumerable<CatalogProduct> products, bool processVariations = true)
        {
            var catalogsMap = _catalogService.GetAllCatalogs().ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);
            var allCategoriesIds = products.Select(x => x.CategoryId).Where(x => x != null).Distinct().ToArray();
            var categoriesMap = _categoryService.GetByIds(allCategoriesIds).ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);

            foreach (var product in products)
            {
                if (string.IsNullOrEmpty(product.Code))
                {
                    product.Code = _skuGenerator.GenerateSku(product);
                }
                product.Catalog = catalogsMap.GetValueOrThrow(product.CatalogId, $"catalog with key {product.CatalogId} doesn't exist");
                if (product.CategoryId != null)
                {
                    product.Category = categoriesMap.GetValueOrThrow(product.CategoryId, $"category with key {product.CategoryId} doesn't exist");
                }

                if (product.Links != null)
                {
                    foreach (var link in product.Links)
                    {
                        link.Catalog = catalogsMap.GetValueOrThrow(link.CatalogId, $"link catalog with key {link.CatalogId} doesn't exist");
                        link.Category = _categoryService.GetByIds(new[] { link.CategoryId }, (CategoryResponseGroup.WithProperties | CategoryResponseGroup.WithParents).ToString()).FirstOrDefault();
                    }
                }

                if (product.MainProduct != null)
                {
                    InnerLoadDependencies(new[] { product.MainProduct }, false);
                }
                if (processVariations && !product.Variations.IsNullOrEmpty())
                {
                    InnerLoadDependencies(product.Variations.ToArray());
                }
                //Resolve relative urls for all product assets
                if (product.AllAssets != null)
                {
                    foreach (var asset in product.AllAssets)
                    {
                        asset.Url = _blobUrlResolver.GetAbsoluteUrl(asset.RelativeUrl);
                    }
                }
            }
        }

        protected virtual void ApplyInheritanceRules(IEnumerable<CatalogProduct> products)
        {
            foreach (var product in products)
            {
                product.TryInheritFrom(product.Category ?? (IEntity)product.Catalog);
                if (product.MainProduct != null)
                {
                    product.TryInheritFrom(product.MainProduct);
                }
            }
        }

        protected virtual void ValidateProducts(IEnumerable<CatalogProduct> products)
        {
            if (products == null)
            {
                throw new ArgumentNullException(nameof(products));
            }

            //Validate products
            var validator = AbstractTypeFactory<ProductValidator>.TryCreateInstance();
            foreach (var product in products)
            {
                validator.ValidateAndThrow(product);
            }
            InnerLoadDependencies(products, false);
            ApplyInheritanceRules(products);

            var targets = products.OfType<IHasProperties>();
            foreach (var item in targets)
            {
                var validatioResult = _hasPropertyValidator.Validate(item);
                if (!validatioResult.IsValid)
                {
                    throw new ValidationException($"Product properties has validation error: {string.Join(Environment.NewLine, validatioResult.Errors.Select(x => x.ToString()))}");
                }
            }
        }
    }
}
