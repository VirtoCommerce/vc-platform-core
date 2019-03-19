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
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CatalogModule.Data.Services
{
    public class ItemServiceImpl : IItemService
    {
        private readonly Func<ICatalogRepository> _repositoryFactory;
        private readonly IEventPublisher _eventPublisher;
        private readonly AbstractValidator<IHasProperties> _hasPropertyValidator;
        private readonly ICatalogService _catalogService;
        private readonly ICategoryService _categoryService;
        private readonly IOutlineService _outlineService;
        private readonly ISeoService _seoService;
        private readonly IPlatformMemoryCache _platformMemoryCache;
        private readonly IBlobUrlResolver _blobUrlResolver;

        public ItemServiceImpl(Func<ICatalogRepository> catalogRepositoryFactory,
                               IEventPublisher eventPublisher, AbstractValidator<IHasProperties> hasPropertyValidator, ICatalogService catalogService, ICategoryService categoryService, IOutlineService outlineService, ISeoService seoService, IPlatformMemoryCache platformMemoryCache
            , IBlobUrlResolver blobUrlResolver)
        {
            _repositoryFactory = catalogRepositoryFactory;
            _eventPublisher = eventPublisher;
            _hasPropertyValidator = hasPropertyValidator;
            _catalogService = catalogService;
            _categoryService = categoryService;
            _outlineService = outlineService;
            _seoService = seoService;
            _platformMemoryCache = platformMemoryCache;
            _blobUrlResolver = blobUrlResolver;
        }

        #region IItemService Members

        public virtual async Task<CatalogProduct[]> GetByIdsAsync(string[] itemIds, ItemResponseGroup respGroup, string catalogId = null)
        {
            var cacheKey = CacheKey.With(GetType(), "GetByIdsAsync", string.Join("-", itemIds), respGroup.ToString(), catalogId);
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                CatalogProduct[] result;

                using (var repository = _repositoryFactory())
                {
                    //Optimize performance and CPU usage
                    repository.DisableChangesTracking();

                    result = (await repository.GetItemByIdsAsync(itemIds, respGroup))
                                       .Select(x => x.ToModel(AbstractTypeFactory<CatalogProduct>.TryCreateInstance()))
                                       .ToArray();
                }

                await LoadDependenciesAsync(result);
                ApplyInheritanceRules(result);

                var productsWithVariationsList = result.Concat(result.Where(p => p.Variations != null)
                                           .SelectMany(p => p.Variations)).ToArray();

                // Fill outlines for products and variations
                if (respGroup.HasFlag(ItemResponseGroup.Outlines))
                {
                    _outlineService.FillOutlinesForObjects(productsWithVariationsList, catalogId);
                }

                // Fill SEO info for products, variations and outline items
                if ((respGroup & ItemResponseGroup.Seo) == ItemResponseGroup.Seo)
                {
                    var objectsWithSeo = productsWithVariationsList.OfType<ISeoSupport>().ToList();
                    //Load SEO information for all Outline.Items
                    var outlineItems = productsWithVariationsList.Where(p => p.Outlines != null)
                                             .SelectMany(p => p.Outlines.SelectMany(o => o.Items));
                    objectsWithSeo.AddRange(outlineItems);
                    await _seoService.LoadSeoForObjectsAsync(objectsWithSeo.ToArray());
                }

                //Reduce details according to response group
                foreach (var product in productsWithVariationsList)
                {
                    ReduceDetails(product, respGroup);
                    cacheEntry.AddExpirationToken(ItemCacheRegion.CreateChangeToken(product));
                }

                return result;
            });
        }

        public virtual async Task<CatalogProduct> GetByIdAsync(string itemId, ItemResponseGroup responseGroup, string catalogId = null)
        {
            var items = await GetByIdsAsync(new[] { itemId }, responseGroup, catalogId);
            return items.FirstOrDefault();
        }

        public virtual async Task SaveChangesAsync(CatalogProduct[] products)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<CatalogProduct>>();

            await ValidateProductsAsync(products);

            using (var repository = _repositoryFactory())
            {
                var dbExistProducts = await repository.GetItemByIdsAsync(products.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
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

                await _eventPublisher.Publish(new ProductChangingEvent(changedEntries));

                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();

                await _eventPublisher.Publish(new ProductChangedEvent(changedEntries));
            }

            ClearCache(products);
        }

        public virtual async Task DeleteAsync(string[] itemIds)
        {
            var items = await GetByIdsAsync(itemIds, ItemResponseGroup.ItemInfo);
            var changedEntries = items
                .Select(i => new GenericChangedEntry<CatalogProduct>(i, EntryState.Deleted))
                .ToList();

            using (var repository = _repositoryFactory())
            {
                await _eventPublisher.Publish(new ProductChangingEvent(changedEntries));

                await repository.RemoveItemsAsync(itemIds);
                await repository.UnitOfWork.CommitAsync();

                await _eventPublisher.Publish(new ProductChangedEvent(changedEntries));
            }

            ClearCache(items);
        }

        #endregion

        protected virtual void ClearCache(IEnumerable<CatalogProduct> entities)
        {
            ItemSearchCacheRegion.ExpireRegion();

            foreach (var entity in entities)
            {
                ItemCacheRegion.ExpireEntity(entity);
            }
        }

        /// <summary>
        /// Reduce product details according to response group
        /// </summary>
        /// <param name="product"></param>
        /// <param name="respGroup"></param>
        protected virtual void ReduceDetails(CatalogProduct product, ItemResponseGroup respGroup)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            if (!respGroup.HasFlag(ItemResponseGroup.ItemAssets))
            {
                product.Assets = null;
            }
            if (!respGroup.HasFlag(ItemResponseGroup.ItemAssociations))
            {
                product.Associations = null;
            }
            if (!respGroup.HasFlag(ItemResponseGroup.ReferencedAssociations))
            {
                product.ReferencedAssociations = null;
            }
            if (!respGroup.HasFlag(ItemResponseGroup.ItemEditorialReviews))
            {
                product.Reviews = null;
            }
            if (!respGroup.HasFlag(ItemResponseGroup.Inventory))
            {
                product.Inventories = null;
            }
            if (!respGroup.HasFlag(ItemResponseGroup.ItemProperties))
            {
                product.Properties = null;
            }
            if (!respGroup.HasFlag(ItemResponseGroup.Links))
            {
                product.Links = null;
            }
            if (!respGroup.HasFlag(ItemResponseGroup.Outlines))
            {
                product.Outlines = null;
            }
            if (!respGroup.HasFlag(ItemResponseGroup.Seo))
            {
                product.SeoInfos = null;
            }
            if (!respGroup.HasFlag(ItemResponseGroup.Variations))
            {
                product.Variations = null;
            }
        }




        protected virtual async Task LoadDependenciesAsync(CatalogProduct[] products, bool processVariations = true)
        {
            var catalogsMap = (await _catalogService.GetCatalogsListAsync()).ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);
            var allCategoriesIds = products.Select(x => x.CategoryId).Where(x => x != null).Distinct().ToArray();
            var categoriesMap = (await _categoryService.GetByIdsAsync(allCategoriesIds, CategoryResponseGroup.Full)).ToDictionary(
                    x => x.Id, StringComparer.OrdinalIgnoreCase);

            var categoryIdsLinks = products.Any(p => !p.Links.IsNullOrEmpty()) ? products.SelectMany(p => p.Links).Select(l => l.CategoryId).Distinct().ToArray() : new string[] { };
            var categoryLinks = await _categoryService.GetByIdsAsync(categoryIdsLinks, CategoryResponseGroup.WithProperties | CategoryResponseGroup.WithParents);

            foreach (var product in products)
            {
                product.Catalog = catalogsMap.GetValueOrThrow(product.CatalogId, $"catalog with key {product.CatalogId} not exist");
                if (product.CategoryId != null)
                {
                    product.Category = categoriesMap.GetValueOrThrow(product.CategoryId, $"category with key {product.CategoryId} not exist");
                }

                if (product.Links != null)
                {
                    foreach (var link in product.Links)
                    {
                        link.Catalog = catalogsMap.GetValueOrThrow(link.CatalogId, $"link catalog with key {link.CatalogId} not exist");
                        link.Category = categoryLinks.FirstOrDefault(cl => cl.Id.EqualsInvariant(link.CategoryId));
                    }
                }

                if (product.MainProduct != null)
                {
                    if (product.MainProduct.MainProduct != null)
                    {
                        throw new OperationCanceledException($"The main product can't contains reference to another main product! It can lead to the infinite recursion.");
                    }
                    await LoadDependenciesAsync(new[] { product.MainProduct }, false);
                }
                if (processVariations && !product.Variations.IsNullOrEmpty())
                {
                    await LoadDependenciesAsync(product.Variations.ToArray());
                }

                if (!product.Images.IsNullOrEmpty())
                {
                    foreach (var image in product.Images)
                    {
                        image.RelativeUrl = image.Url;
                        image.Url = _blobUrlResolver.GetAbsoluteUrl(image.Url);
                    }
                }

            }
        }

        protected virtual void ApplyInheritanceRules(CatalogProduct[] products, bool processVariations = true)
        {
            foreach (var product in products)
            {
                //Inherit images from parent product (if its not set)
                if (product.Images.IsNullOrEmpty() && product.MainProduct != null && !product.MainProduct.Images.IsNullOrEmpty())
                {
                    product.Images = product.MainProduct.Images.Select(x => x.Clone()).OfType<Image>().ToList();
                    foreach (var image in product.Images)
                    {
                        image.Id = null;
                        image.IsInherited = true;
                    }
                }

                //Inherit assets from parent product (if its not set)
                if (product.Assets.IsNullOrEmpty() && product.MainProduct != null && product.MainProduct.Assets != null)
                {
                    product.Assets = product.MainProduct.Assets.Select(x => x.Clone()).OfType<Asset>().ToList();
                    foreach (var asset in product.Assets)
                    {
                        asset.Id = null;
                        asset.IsInherited = true;
                    }
                }

                //inherit editorial reviews from main product and do not inherit if variation loaded within product
                if (processVariations && product.Reviews.IsNullOrEmpty() && product.MainProduct != null && product.MainProduct.Reviews != null)
                {
                    product.Reviews = product.MainProduct.Reviews.Select(x => x.Clone()).OfType<EditorialReview>().ToList();
                    foreach (var review in product.Reviews)
                    {
                        review.Id = null;
                        review.IsInherited = true;
                    }
                }

                //TaxType category inheritance
                if (product.TaxType == null && product.Category != null)
                {
                    product.TaxType = product.Category.TaxType;
                }

                //Properties inheritance
                var catalogProperties = (product.Category != null ? product.Category.Properties : product.Catalog.Properties)
                    .Select(x => { x.IsInherited = true; x.IsReadOnly = false; return x.Clone(); })
                    .OfType<Property>()
                    .OrderBy(x => x.Name).ToList();

                var unionProperties = !product.Properties.IsNullOrEmpty()
                    ? catalogProperties.Union(product.Properties.Where(prp => !catalogProperties.Select(cp => cp.Name).Contains(prp.Name))).ToList()
                    : catalogProperties;

                if (!unionProperties.IsNullOrEmpty())
                {
                    foreach (var property in catalogProperties)
                    {
                        property.IsInherited = true;

                        if (!property.ValidationRules.IsNullOrEmpty())
                        {
                            foreach (var validationRule in property.ValidationRules)
                            {
                                if (validationRule.Property == null)
                                {
                                    validationRule.Property = property;
                                }
                            }
                        }
                    }

                    product.Properties = unionProperties.Select(up =>
                    {
                        if (up.Catalog == null)
                        {
                            up.Catalog = product.Catalog;
                            up.CatalogId = product.CatalogId;
                        }

                        if (!product.Properties.IsNullOrEmpty())
                        {
                            //TODO 
                            up.Values = product.Properties.Where(p => p.Name.EqualsInvariant(up.Name)/* && p.Values.All(v => v.ValueType == up.ValueType)*/)
                                .SelectMany(pv => pv.Values).Select(val =>
                                {
                                    val.Property = up;
                                    val.PropertyId = up.Id;
                                    val.PropertyName = up.Name;
                                    val.ValueType = up.ValueType;
                                    return val;
                                }).ToList();
                        }
                        return up;
                    }).ToArray();
                }

                //inherit not overridden property values from main product
                if (product.MainProduct != null && !product.MainProduct.Properties.IsNullOrEmpty())
                {
                    var mainProductPopValuesGroups = product.MainProduct.Properties.GroupBy(x => x.Name);
                    foreach (var group in mainProductPopValuesGroups)
                    {
                        //Inherit all values if not overriden
                        foreach (var property in product.Properties)
                        {
                            if (!property.Values.Any(x => x.PropertyName.EqualsInvariant(group.Key)))
                            {
                                foreach (var inheritedpropValue in group.Select(x => x.Clone()).OfType<PropertyValue>())
                                {
                                    inheritedpropValue.Id = null;
                                    inheritedpropValue.IsInherited = true;
                                    property.Values.Add(inheritedpropValue);
                                }
                            }
                        }

                    }
                }

                //Measurement inheritance 
                if (product.MainProduct != null)
                {
                    product.Width = product.Width ?? product.MainProduct.Width;
                    product.Height = product.Height ?? product.MainProduct.Height;
                    product.Length = product.Length ?? product.MainProduct.Length;
                    product.MeasureUnit = product.MeasureUnit ?? product.MainProduct.MeasureUnit;
                    product.Weight = product.Weight ?? product.MainProduct.Weight;
                    product.WeightUnit = product.WeightUnit ?? product.MainProduct.WeightUnit;
                    product.PackageType = product.PackageType ?? product.MainProduct.PackageType;
                }

                if (processVariations && !product.Variations.IsNullOrEmpty())
                {
                    ApplyInheritanceRules(product.Variations.ToArray(), processVariations: false);
                }
            }
        }

        protected virtual async Task ValidateProductsAsync(CatalogProduct[] products)
        {
            if (products == null)
            {
                throw new ArgumentNullException(nameof(products));
            }

            //Validate products
            var validator = new ProductValidator();
            foreach (var product in products)
            {
                validator.ValidateAndThrow(product);
            }

            await LoadDependenciesAsync(products, false);
            ApplyInheritanceRules(products, false);

            var targets = products.OfType<IHasProperties>();
            foreach (var item in targets)
            {
                var validatioResult = await _hasPropertyValidator.ValidateAsync(item);
                if (!validatioResult.IsValid)
                {
                    throw new ValidationException($"Product properties has validation error: {string.Join(Environment.NewLine, validatioResult.Errors.Select(x => x.ToString()))}");
                }
            }
        }
    }
}
