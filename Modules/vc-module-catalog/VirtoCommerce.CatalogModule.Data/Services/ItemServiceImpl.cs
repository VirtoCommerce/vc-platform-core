using System;
using System.Collections.Generic;
using System.Linq;
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
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CatalogModule.Data.Services
{
    public class ItemServiceImpl : ServiceBase, IItemService
    {
        private readonly ICategoryService _categoryService;
        private readonly ICatalogService _catalogService;
        private readonly ICommerceService _commerceService;
        private readonly IOutlineService _outlineService;
        private readonly Func<ICatalogRepository> _repositoryFactory;
        private readonly AbstractValidator<IHasProperties> _hasPropertyValidator;
        private readonly IEventPublisher _eventPublisher;

        public ItemServiceImpl(Func<ICatalogRepository> catalogRepositoryFactory, ICommerceService commerceService, IOutlineService outlineService, ICatalogService catalogService, ICategoryService categoryService, AbstractValidator<IHasProperties> hasPropertyValidator,
                               IEventPublisher eventPublisher)
        {
            _catalogService = catalogService;
            _categoryService = categoryService;
            _commerceService = commerceService;
            _outlineService = outlineService;
            _repositoryFactory = catalogRepositoryFactory;
            _hasPropertyValidator = hasPropertyValidator;
            _eventPublisher = eventPublisher;
        }

        #region IItemService Members

        public virtual CatalogProduct GetById(string itemId, ItemResponseGroup respGroup, string catalogId = null)
        {
            var results = GetByIds(new[] { itemId }, respGroup, catalogId);
            return results.Any() ? results.First() : null;
        }

        public virtual CatalogProduct[] GetByIds(string[] itemIds, ItemResponseGroup respGroup, string catalogId = null)
        {
            CatalogProduct[] result;

            using (var repository = _repositoryFactory())
            {
                //Optimize performance and CPU usage
                repository.DisableChangesTracking();

                result = repository.GetItemByIds(itemIds, respGroup)
                                   .Select(x => x.ToModel(AbstractTypeFactory<CatalogProduct>.TryCreateInstance()))
                                   .ToArray();
            }

            LoadDependencies(result);
            ApplyInheritanceRules(result);

            var productsWithVariationsList = result.Concat(result.Where(p => p.Variations != null)
                                       .SelectMany(p => p.Variations));
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
                _commerceService.LoadSeoForObjects(objectsWithSeo.ToArray());
            }

            //Reduce details according to response group
            foreach (var product in productsWithVariationsList)
            {
                ReduceDetails(product, respGroup);
            }

            return result;
        }

        public virtual void Create(CatalogProduct[] items)
        {
            SaveChanges(items);
        }

        public virtual CatalogProduct Create(CatalogProduct item)
        {
            Create(new[] { item });
            var retVal = GetById(item.Id, ItemResponseGroup.ItemLarge);
            return retVal;
        }

        public virtual void Update(CatalogProduct[] items)
        {
            SaveChanges(items);
        }

        public virtual void Delete(string[] itemIds)
        {
            var items = GetByIds(itemIds, ItemResponseGroup.ItemInfo);
            var changedEntries = items
                .Select(i => new GenericChangedEntry<CatalogProduct>(i, EntryState.Deleted))
                .ToList();

            using (var repository = _repositoryFactory())
            {
                _eventPublisher.Publish(new ProductChangingEvent(changedEntries));

                repository.RemoveItems(itemIds);
                CommitChanges(repository);

                _eventPublisher.Publish(new ProductChangedEvent(changedEntries));
            }
        }

        #endregion
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
                product.PropertyValues = null;
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

        protected virtual void SaveChanges(CatalogProduct[] products, bool disableValidation = false)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<CatalogProduct>>();

            ValidateProducts(products);

            using (var repository = _repositoryFactory())
            using (var changeTracker = GetChangeTracker(repository))
            {
                var dbExistProducts = repository.GetItemByIds(products.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray(), ItemResponseGroup.ItemLarge);
                foreach (var product in products)
                {
                    var modifiedEntity = AbstractTypeFactory<ItemEntity>.TryCreateInstance().FromModel(product, pkMap);
                    var originalEntity = dbExistProducts.FirstOrDefault(x => x.Id == product.Id);

                    if (originalEntity != null)
                    {
                        changeTracker.Attach(originalEntity);
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

                _eventPublisher.Publish(new ProductChangingEvent(changedEntries));

                CommitChanges(repository);
                pkMap.ResolvePrimaryKeys();

                _eventPublisher.Publish(new ProductChangedEvent(changedEntries));
            }

            //Update SEO 
            var productsWithVariations = products.Concat(products.Where(x => x.Variations != null).SelectMany(x => x.Variations)).OfType<ISeoSupport>().ToArray();
            _commerceService.UpsertSeoForObjects(productsWithVariations);
        }


        protected virtual void LoadDependencies(CatalogProduct[] products, bool processVariations = true)
        {
            var catalogsMap = _catalogService.GetCatalogsList().ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);
            var allCategoriesIds = products.Select(x => x.CategoryId).Where(x => x != null).Distinct().ToArray();
            var categoriesMap = _categoryService.GetByIds(allCategoriesIds, CategoryResponseGroup.Full).ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);

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
                        link.Category = _categoryService.GetById(link.CategoryId, CategoryResponseGroup.WithProperties | CategoryResponseGroup.WithParents);
                    }
                }

                if (product.MainProduct != null)
                {
                    if (product.MainProduct.MainProduct != null)
                    {
                        throw new OperationCanceledException($"The main product can't contains reference to another main product! It can lead to the infinite recursion.");
                    }
                    LoadDependencies(new[] { product.MainProduct }, false);
                }
                if (processVariations && !product.Variations.IsNullOrEmpty())
                {
                    LoadDependencies(product.Variations.ToArray());
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
                product.Properties = (product.Category != null ? product.Category.Properties : product.Catalog.Properties).Select(x => x.Clone())
                                     .OfType<Property>()
                                     .OrderBy(x => x.Name)
                                     .ToList();

                if (!product.Properties.IsNullOrEmpty())
                {
                    foreach (var property in product.Properties)
                    {
                        property.IsInherited = true;

                        if (property.ValidationRules == null) continue;
                        foreach (var validationRule in property.ValidationRules)
                        {
                            if (validationRule.Property == null)
                            {
                                validationRule.Property = property;
                            }
                        }
                    }
                }

                if (!product.PropertyValues.IsNullOrEmpty())
                {
                    //Self item property values
                    foreach (var propertyValue in product.PropertyValues.ToArray())
                    {
                        //Try to find property meta information
                        propertyValue.Property = product.Properties.Where(x => x.Type == PropertyType.Product || x.Type == PropertyType.Variation)
                                                                   .FirstOrDefault(x => x.IsSuitableForValue(propertyValue));
                    }
                }

                //inherit not overridden property values from main product
                if (product.MainProduct != null && !product.MainProduct.PropertyValues.IsNullOrEmpty())
                {
                    var mainProductPopValuesGroups = product.MainProduct.PropertyValues.GroupBy(x => x.PropertyName);
                    foreach (var group in mainProductPopValuesGroups)
                    {
                        //Inherit all values if not overriden
                        if (!product.PropertyValues.Any(x => x.PropertyName.EqualsInvariant(group.Key)))
                        {
                            foreach (var inheritedpropValue in group.Select(x => x.Clone()).OfType<PropertyValue>())
                            {
                                inheritedpropValue.Id = null;
                                inheritedpropValue.IsInherited = true;
                                product.PropertyValues.Add(inheritedpropValue);
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

        protected virtual void ValidateProducts(CatalogProduct[] products)
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

            LoadDependencies(products, false);
            ApplyInheritanceRules(products, false);

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
