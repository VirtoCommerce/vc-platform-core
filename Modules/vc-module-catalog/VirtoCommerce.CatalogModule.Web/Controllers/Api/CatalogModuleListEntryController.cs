using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Web.Controllers.Api
{
    [Route("api/catalog/listentries")]
    public class CatalogModuleListEntryController : Controller
    {
        private readonly ICatalogSearchService _searchService;
        private readonly ICategoryService _categoryService;
        private readonly ICatalogService _catalogService;
        private readonly IItemService _itemService;

        public CatalogModuleListEntryController(
            ICatalogSearchService searchService,
            ICategoryService categoryService,
            IItemService itemService,
            ICatalogService catalogService
            )
        {
            _searchService = searchService;
            _categoryService = categoryService;
            _itemService = itemService;
            _catalogService = catalogService;
        }

        /// <summary>
        /// Searches for the items by complex criteria.
        /// </summary>
        /// <param name="criteria">The search criteria.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<ListEntrySearchResult>> ListItemsSearchAsync([FromBody]CatalogListEntrySearchCriteria criteria)
        {
            //ApplyRestrictionsForCurrentUser(coreModelCriteria);

            criteria.WithHidden = true;
            //Need search in children categories if user specify keyword
            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                criteria.SearchInChildren = true;
                criteria.SearchInVariations = true;
            }

            var retVal = new ListEntrySearchResult();

            var categorySkip = 0;
            var categoryTake = 0;
            //Because products and categories represent in search result as two separated collections for handle paging request 
            //we should join two resulting collection artificially
            //search categories
            var copyRespGroup = criteria.ResponseGroup;
            if ((criteria.ResponseGroup & SearchResponseGroup.WithCategories) == SearchResponseGroup.WithCategories)
            {
                criteria.ResponseGroup = criteria.ResponseGroup & ~SearchResponseGroup.WithProducts;
                var categoriesSearchResult = await _searchService.SearchAsync(criteria);
                var categoriesTotalCount = categoriesSearchResult.Categories.Count;

                categorySkip = Math.Min(categoriesTotalCount, criteria.Skip);
                categoryTake = Math.Min(criteria.Take, Math.Max(0, categoriesTotalCount - criteria.Skip));
                var categories = categoriesSearchResult.Categories.Skip(categorySkip).Take(categoryTake).Select(x => new ListEntryCategory(x)).ToList();

                retVal.TotalCount = categoriesTotalCount;
                retVal.ListEntries.AddRange(categories);
            }
            criteria.ResponseGroup = copyRespGroup;
            //search products
            if ((criteria.ResponseGroup & SearchResponseGroup.WithProducts) == SearchResponseGroup.WithProducts)
            {
                criteria.ResponseGroup = criteria.ResponseGroup & ~SearchResponseGroup.WithCategories;
                criteria.Skip = criteria.Skip - categorySkip;
                criteria.Take = criteria.Take - categoryTake;
                var productsSearchResult = await _searchService.SearchAsync(criteria);

                var products = productsSearchResult.Products.Select(x => new ListEntryProduct(x));

                retVal.TotalCount += productsSearchResult.ProductsTotalCount;
                retVal.ListEntries.AddRange(products);
            }


            return Ok(retVal);
        }

        /// <summary>
        /// Creates links for categories or items to parent categories and catalogs.
        /// </summary>
        /// <param name="links">The links.</param>
        [HttpPost]
        [Route("~/api/catalog/listentrylinks")]
        public async Task<ActionResult> CreateLinks([FromBody]ListEntryLink[] links)
        {
            //Scope bound security check
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Create, links);

            await InnerUpdateLinks(links, (x, y) => x.Links.Add(y));
            return NoContent();
        }

        /// <summary>
        /// Bulk create links to categories and items
        /// </summary>
        /// <param name="creationRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("~/api/catalog/listentrylinks/bulkcreate")]
        public async Task<IActionResult> BulkCreateLinks([FromBody]BulkLinkCreationRequest creationRequest)
        {

            if (creationRequest.CatalogId.IsNullOrEmpty() || creationRequest.CategoryId.IsNullOrEmpty())
            {
                throw new ArgumentException("Target catalog and category identifiers should be specified.");
            }

            var coreModelCriteria = creationRequest.SearchCriteria;

            bool haveProducts;

            do
            {
                var links = new List<ListEntryLink>();

                var searchResult = await _searchService.SearchAsync(coreModelCriteria);

                var productLinks = searchResult
                    .Products
                    .Select(x => new ListEntryLink
                    {
                        CatalogId = creationRequest.CatalogId,
                        ListEntryType = ListEntryProduct.TypeName,
                        ListEntryId = x.Id,
                        CategoryId = creationRequest.CategoryId
                    })
                    .ToList();

                links.AddRange(productLinks);

                if (coreModelCriteria.ResponseGroup.HasFlag(SearchResponseGroup.WithCategories))
                {
                    coreModelCriteria.ResponseGroup = coreModelCriteria.ResponseGroup & ~SearchResponseGroup.WithCategories;

                    var categoryLinks = searchResult
                        .Categories
                        .Select(c => new ListEntryLink
                        {
                            CatalogId = creationRequest.CatalogId,
                            ListEntryType = ListEntryCategory.TypeName,
                            ListEntryId = c.Id,
                            CategoryId = creationRequest.CategoryId
                        })
                        .ToList();

                    links.AddRange(categoryLinks);
                }

                haveProducts = productLinks.Any();

                coreModelCriteria.Skip += coreModelCriteria.Take;

                //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Create, links.ToArray());

                await InnerUpdateLinks(links.ToArray(), (x, y) => x.Links.Add(y));

            } while (haveProducts);

            return NoContent();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        [Route("~/api/catalog/getslug")]
        public ActionResult<string> GetSlug(string text)
        {
            //if (text == null)
            //{
            //    return InternalServerError(new NullReferenceException("text"));
            //}
            return Ok(text.GenerateSlug());
        }

        /// <summary>
        /// Unlinks the linked categories or items from parent categories and catalogs.
        /// </summary>
        /// <param name="links">The links.</param>
        [HttpPost]
        [Route("~/api/catalog/listentrylinks/delete")]
        public async Task<IActionResult> DeleteLinks([FromBody]ListEntryLink[] links)
        {
            //Scope bound security check
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Delete, links);

            await InnerUpdateLinks(links, (x, y) => x.Links = x.Links.Where(l => string.Join(":", l.CatalogId, l.CategoryId) != string.Join(":", y.CatalogId, y.CategoryId)).ToList());
            return NoContent();
        }

        /// <summary>
        /// Move categories or products to another location.
        /// </summary>
        /// <param name="moveInfo">Move operation details</param>
        [HttpPost]
        [Route("move")]
        public async Task<IActionResult> Move([FromBody]MoveInfo moveInfo)
        {
            var categories = new List<Category>();
            var dstCatalog = (await _catalogService.GetByIdsAsync(new[] { moveInfo.Catalog })).FirstOrDefault();
            if (dstCatalog != null && dstCatalog.IsVirtual)
            {
                throw new InvalidOperationException("Unable to move in virtual catalog");
            }

            //Move  categories
            foreach (var listEntryCategory in moveInfo.ListEntries.Where(x => x.Type.EqualsInvariant(ListEntryCategory.TypeName)))
            {
                var category = (await _categoryService.GetByIdsAsync(new[] { listEntryCategory.Id }, CategoryResponseGroup.Info)).FirstOrDefault();
                if (category != null && category.CatalogId != moveInfo.Catalog)
                {
                    category.CatalogId = moveInfo.Catalog;
                }
                if (category != null && category.ParentId != moveInfo.Category)
                {
                    category.ParentId = moveInfo.Category;
                }
                categories.Add(category);
            }

            var products = new List<CatalogProduct>();
            //Move products
            foreach (var listEntryProduct in moveInfo.ListEntries.Where(x => x.Type.EqualsInvariant(ListEntryProduct.TypeName)))
            {
                var product = (await _itemService.GetByIdsAsync(new[] { listEntryProduct.Id }, ItemResponseGroup.ItemLarge)).FirstOrDefault();
                if (product != null && product.CatalogId != moveInfo.Catalog)
                {
                    product.CatalogId = moveInfo.Catalog;
                    product.CategoryId = null;
                    foreach (var variation in product.Variations)
                    {
                        variation.CatalogId = moveInfo.Catalog;
                        variation.CategoryId = null;
                    }

                }
                if (product != null && product.CategoryId != moveInfo.Category)
                {
                    product.CategoryId = moveInfo.Category;
                    foreach (var variation in product.Variations)
                    {
                        variation.CategoryId = moveInfo.Category;
                    }
                }
                products.Add(product);

                if (product != null)
                {
                    products.AddRange(product.Variations);
                }
            }

            //Scope bound security check
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Create, categories);
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Create, products);

            if (categories.Any())
            {
                await _categoryService.SaveChangesAsync(categories.ToArray());
            }
            if (products.Any())
            {
                await _itemService.SaveChangesAsync(products.ToArray());
            }
            return Ok();
        }

        private async Task InnerUpdateLinks(ListEntryLink[] links, Action<ILinkSupport, CategoryLink> action)
        {
            var changedObjects = new List<ILinkSupport>();
            foreach (var link in links)
            {
                ILinkSupport changedObject;
                var newlink = new CategoryLink
                {
                    CategoryId = link.CategoryId,
                    CatalogId = link.CatalogId
                };

                if (link.ListEntryType.EqualsInvariant(ListEntryCategory.TypeName))
                {
                    changedObject = (await _categoryService.GetByIdsAsync(new[] { link.ListEntryId }, CategoryResponseGroup.Full)).FirstOrDefault();
                }
                else
                {
                    changedObject = (await _itemService.GetByIdsAsync(new[] { link.ListEntryId }, ItemResponseGroup.ItemLarge)).FirstOrDefault();
                }
                action(changedObject, newlink);
                changedObjects.Add(changedObject);
            }

            var categorySaveChangesTask = _categoryService.SaveChangesAsync(changedObjects.OfType<Category>().ToArray());
            var itemSaveChangesTask = _itemService.SaveChangesAsync(changedObjects.OfType<CatalogProduct>().ToArray());

            await Task.WhenAll(categorySaveChangesTask, itemSaveChangesTask);
        }
    }
}
