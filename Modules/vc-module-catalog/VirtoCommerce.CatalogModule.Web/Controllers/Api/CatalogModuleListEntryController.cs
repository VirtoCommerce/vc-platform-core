using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CatalogModule.Web.Converters;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using coreModel = VirtoCommerce.CatalogModule.Core.Model;
using webModel = VirtoCommerce.CatalogModule.Web.Model;

namespace VirtoCommerce.CatalogModule.Web.Controllers.Api
{
    [Route("api/catalog/listentries")]
    public class CatalogModuleListEntryController : Controller
    {
        private readonly ICatalogSearchService _searchService;
        private readonly ICategoryService _categoryService;
        private readonly ICatalogService _catalogService;
        private readonly IItemService _itemService;
        private readonly IBlobUrlResolver _blobUrlResolver;

        public CatalogModuleListEntryController(
            ICatalogSearchService searchService,
            ICategoryService categoryService,
            IItemService itemService,
            IBlobUrlResolver blobUrlResolver,
            ICatalogService catalogService
            )
        {
            _searchService = searchService;
            _categoryService = categoryService;
            _itemService = itemService;
            _blobUrlResolver = blobUrlResolver;
            _catalogService = catalogService;
        }

        /// <summary>
        /// Searches for the items by complex criteria.
        /// </summary>
        /// <param name="criteria">The search criteria.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<webModel.ListEntrySearchResult>> ListItemsSearchAsync([FromBody]webModel.SearchCriteria criteria)
        {
            var coreModelCriteria = criteria.ToCoreModel();
            //ApplyRestrictionsForCurrentUser(coreModelCriteria);

            coreModelCriteria.WithHidden = true;
            //Need search in children categories if user specify keyword
            if (!string.IsNullOrEmpty(coreModelCriteria.Keyword))
            {
                coreModelCriteria.SearchInChildren = true;
                coreModelCriteria.SearchInVariations = true;
            }

            var retVal = new webModel.ListEntrySearchResult();

            var categorySkip = 0;
            var categoryTake = 0;
            //Because products and categories represent in search result as two separated collections for handle paging request 
            //we should join two resulting collection artificially
            //search categories
            var copyRespGroup = coreModelCriteria.ResponseGroup;
            if ((coreModelCriteria.ResponseGroup & coreModel.Search.SearchResponseGroup.WithCategories) == coreModel.Search.SearchResponseGroup.WithCategories)
            {
                coreModelCriteria.ResponseGroup = coreModelCriteria.ResponseGroup & ~coreModel.Search.SearchResponseGroup.WithProducts;
                var categoriesSearchResult = await _searchService.SearchAsync(coreModelCriteria);
                var categoriesTotalCount = categoriesSearchResult.Categories.Count();

                categorySkip = Math.Min(categoriesTotalCount, coreModelCriteria.Skip);
                categoryTake = Math.Min(coreModelCriteria.Take, Math.Max(0, categoriesTotalCount - coreModelCriteria.Skip));
                var categories = categoriesSearchResult.Categories.Skip(categorySkip).Take(categoryTake).Select(x => new webModel.ListEntryCategory(x.ToWebModel(_blobUrlResolver))).ToList();

                retVal.TotalCount = categoriesTotalCount;
                retVal.ListEntries.AddRange(categories);
            }
            coreModelCriteria.ResponseGroup = copyRespGroup;
            //search products
            if ((coreModelCriteria.ResponseGroup & coreModel.Search.SearchResponseGroup.WithProducts) == coreModel.Search.SearchResponseGroup.WithProducts)
            {
                coreModelCriteria.ResponseGroup = coreModelCriteria.ResponseGroup & ~coreModel.Search.SearchResponseGroup.WithCategories;
                coreModelCriteria.Skip = coreModelCriteria.Skip - categorySkip;
                coreModelCriteria.Take = coreModelCriteria.Take - categoryTake;
                var productsSearchResult = await _searchService.SearchAsync(coreModelCriteria);

                var products = productsSearchResult.Products.Select(x => new webModel.ListEntryProduct(x.ToWebModel(_blobUrlResolver)));

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
        public async Task<ActionResult> CreateLinks([FromBody]webModel.ListEntryLink[] links)
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
        public async Task<IActionResult> BulkCreateLinks(webModel.BulkLinkCreationRequest creationRequest)
        {

            if (creationRequest.CatalogId.IsNullOrEmpty() || creationRequest.CategoryId.IsNullOrEmpty())
            {
                throw new ArgumentException("Target catalog and category identifiers should be specified.");
            }

            var coreModelCriteria = creationRequest.SearchCriteria.ToCoreModel();

            bool haveProducts;

            do
            {
                var links = new List<webModel.ListEntryLink>();

                var searchResult = await _searchService.SearchAsync(coreModelCriteria);

                var productLinks = searchResult
                    .Products
                    .Select(x => new webModel.ListEntryLink
                    {
                        CatalogId = creationRequest.CatalogId,
                        ListEntryType = webModel.ListEntryProduct.TypeName,
                        ListEntryId = x.Id,
                        CategoryId = creationRequest.CategoryId
                    })
                    .ToList();

                links.AddRange(productLinks);

                if (coreModelCriteria.ResponseGroup.HasFlag(coreModel.Search.SearchResponseGroup.WithCategories))
                {
                    coreModelCriteria.ResponseGroup = coreModelCriteria.ResponseGroup & ~coreModel.Search.SearchResponseGroup.WithCategories;

                    var categoryLinks = searchResult
                        .Categories
                        .Select(c => new webModel.ListEntryLink
                        {
                            CatalogId = creationRequest.CatalogId,
                            ListEntryType = webModel.ListEntryCategory.TypeName,
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
        public async Task<IActionResult> DeleteLinks(webModel.ListEntryLink[] links)
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
        public async Task<IActionResult> Move(webModel.MoveInfo moveInfo)
        {
            var categories = new List<coreModel.Category>();
            var dstCatalog = (await _catalogService.GetByIdsAsync(new[] { moveInfo.Catalog })).FirstOrDefault();
            if (dstCatalog.IsVirtual)
            {
                throw new InvalidOperationException("Unable to move in virtual catalog");
            }

            //Move  categories
            foreach (var listEntryCategory in moveInfo.ListEntries.Where(x => x.Type.EqualsInvariant(webModel.ListEntryCategory.TypeName)))
            {
                var category = (await _categoryService.GetByIdsAsync(new[] { listEntryCategory.Id }, coreModel.CategoryResponseGroup.Info)).FirstOrDefault();
                if (category.CatalogId != moveInfo.Catalog)
                {
                    category.CatalogId = moveInfo.Catalog;
                }
                if (category.ParentId != moveInfo.Category)
                {
                    category.ParentId = moveInfo.Category;
                }
                categories.Add(category);
            }

            var products = new List<coreModel.CatalogProduct>();
            //Move products
            foreach (var listEntryProduct in moveInfo.ListEntries.Where(x => x.Type.EqualsInvariant(webModel.ListEntryProduct.TypeName)))
            {
                var product = (await _itemService.GetByIdsAsync(new[] { listEntryProduct.Id }, coreModel.ItemResponseGroup.ItemLarge)).FirstOrDefault();
                if (product.CatalogId != moveInfo.Catalog)
                {
                    product.CatalogId = moveInfo.Catalog;
                    product.CategoryId = null;
                    foreach (var variation in product.Variations)
                    {
                        variation.CatalogId = moveInfo.Catalog;
                        variation.CategoryId = null;
                    }

                }
                if (product.CategoryId != moveInfo.Category)
                {
                    product.CategoryId = moveInfo.Category;
                    foreach (var variation in product.Variations)
                    {
                        variation.CategoryId = moveInfo.Category;
                    }
                }
                products.Add(product);
                products.AddRange(product.Variations);
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

        private async Task InnerUpdateLinks(webModel.ListEntryLink[] links, Action<coreModel.ILinkSupport, coreModel.CategoryLink> action)
        {
            var changedObjects = new List<coreModel.ILinkSupport>();
            foreach (var link in links)
            {
                coreModel.ILinkSupport changedObject;
                var newlink = new coreModel.CategoryLink
                {
                    CategoryId = link.CategoryId,
                    CatalogId = link.CatalogId
                };

                if (link.ListEntryType.EqualsInvariant(webModel.ListEntryCategory.TypeName))
                {
                    changedObject = (await _categoryService.GetByIdsAsync(new[] { link.ListEntryId }, coreModel.CategoryResponseGroup.Full)).FirstOrDefault();
                }
                else
                {
                    changedObject = (await _itemService.GetByIdsAsync(new[] { link.ListEntryId }, coreModel.ItemResponseGroup.ItemLarge)).FirstOrDefault();
                }
                action(changedObject, newlink);
                changedObjects.Add(changedObject);
            }

            var categorySaveChangesTask = _categoryService.SaveChangesAsync(changedObjects.OfType<coreModel.Category>().ToArray());
            var itemSaveChangesTask = _itemService.SaveChangesAsync(changedObjects.OfType<coreModel.CatalogProduct>().ToArray());

            await Task.WhenAll(categorySaveChangesTask, itemSaveChangesTask);
        }
    }
}
