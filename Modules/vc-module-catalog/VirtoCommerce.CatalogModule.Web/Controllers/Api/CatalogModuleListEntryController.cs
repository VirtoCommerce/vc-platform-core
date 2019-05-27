using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.CatalogModule.Core;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.ListEntry;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CatalogModule.Web.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Web.Controllers.Api
{
    [Route("api/catalog/listentries")]
    public class CatalogModuleListEntryController : Controller
    {
        private readonly IProductIndexedSearchService _productIndexedSearchService;
        private readonly ICategoryService _categoryService;
        private readonly ICatalogService _catalogService;
        private readonly IItemService _itemService;
        private readonly IListEntrySearchService _listEntrySearchService;
        private readonly ISettingsManager _settingsManager;

        public CatalogModuleListEntryController(IProductIndexedSearchService productIndexedSearchService, IListEntrySearchService listEntrySearchService,
            ICategoryService categoryService, IItemService itemService, ICatalogService catalogService, ISettingsManager settingsManager)
        {
            _productIndexedSearchService = productIndexedSearchService;
            _categoryService = categoryService;
            _itemService = itemService;
            _catalogService = catalogService;
            _listEntrySearchService = listEntrySearchService;
            _settingsManager = settingsManager;
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
            //TODO:
            //ApplyRestrictionsForCurrentUser(coreModelCriteria);
            var result = new ListEntrySearchResult();
            var useIndexedSearch = _settingsManager.GetValue(ModuleConstants.Settings.Search.UseCatalogIndexedSearchInManager.Name, true);

            if (useIndexedSearch && !string.IsNullOrEmpty(criteria.Keyword))
            {
                // TODO: create outline for category
                // TODO: implement sorting

                const ItemResponseGroup responseGroup = ItemResponseGroup.ItemInfo | ItemResponseGroup.Outlines;

                var productIndexedSearchCriteria = AbstractTypeFactory<ProductIndexedSearchCriteria>.TryCreateInstance();

                productIndexedSearchCriteria.ObjectType = KnownDocumentTypes.Product;
                productIndexedSearchCriteria.Keyword = criteria.Keyword;
                productIndexedSearchCriteria.CatalogId = criteria.CatalogId;
                productIndexedSearchCriteria.Outline = criteria.CategoryId;
                productIndexedSearchCriteria.WithHidden = !criteria.HideDirectLinkedCategories;
                productIndexedSearchCriteria.Skip = criteria.Skip;
                productIndexedSearchCriteria.Take = criteria.Take;
                productIndexedSearchCriteria.ResponseGroup = responseGroup.ToString();
                productIndexedSearchCriteria.Sort = criteria.Sort;

                var indexedSearchResult = await _productIndexedSearchService.SearchAsync(productIndexedSearchCriteria);
                result.TotalCount = (int)indexedSearchResult.TotalCount;
                result.Results = indexedSearchResult.Items.Select(x => AbstractTypeFactory<ProductListEntry>.TryCreateInstance().FromModel(x)).ToList();
            }
            else
            {
                result = await _listEntrySearchService.SearchAsync(criteria);
            }
            return Ok(result);
        }


        /// <summary>
        /// Creates links for categories or items to parent categories and catalogs.
        /// </summary>
        /// <param name="links">The links.</param>
        [HttpPost]
        [Route("~/api/catalog/listentrylinks")]
        public async Task<ActionResult> CreateLinks([FromBody] CategoryLink[] links)
        {
            //TODO:
            //Scope bound security check
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Create, links);
            var entryIds = links.Select(x => x.EntryId).ToArray();
            var hasLinkEntries = await LoadCatalogEntriesAsync<IHasLinks>(entryIds);
            foreach (var link in links)
            {
                var hasLinkEntry = hasLinkEntries.FirstOrDefault(x => x.Id.Equals(link.EntryId));
                if (hasLinkEntry != null && !hasLinkEntry.Links.Contains(link))
                {
                    hasLinkEntry.Links.Add(link);
                }
            }
            if (!hasLinkEntries.IsNullOrEmpty())
            {
                await SaveListCatalogEntitiesAsync(hasLinkEntries.ToArray());
            }
            return NoContent();
        }



        /// <summary>
        /// Bulk create links to categories and items
        /// </summary>
        /// <param name="creationRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("~/api/catalog/listentrylinks/bulkcreate")]
        public async Task<ActionResult> BulkCreateLinks([FromBody]BulkLinkCreationRequest creationRequest)
        {
            if (creationRequest.CatalogId.IsNullOrEmpty() || creationRequest.CategoryId.IsNullOrEmpty())
            {
                return BadRequest("Target catalog and category identifiers should be specified.");
            }

            var searchCriteria = creationRequest.SearchCriteria;

            bool haveProducts;

            do
            {
                var links = new List<IHasLinks>();

                var searchResult = await _listEntrySearchService.SearchAsync(searchCriteria);
                var hasLinksEntities = await LoadCatalogEntriesAsync<IHasLinks>(searchResult.ListEntries.Select(x => x.Id).ToArray());
                haveProducts = hasLinksEntities.Any();

                searchCriteria.Skip += searchCriteria.Take;

                //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Create, links.ToArray());
                if (haveProducts)
                {
                    await SaveListCatalogEntitiesAsync(hasLinksEntities.ToArray());
                }

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
        public async Task<ActionResult> DeleteLinks([FromBody]CategoryLink[] links)
        {
            //Scope bound security check
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Delete, links);

            var entryIds = links.Select(x => x.EntryId).ToArray();
            var hasLinkEntries = await LoadCatalogEntriesAsync<IHasLinks>(entryIds);
            foreach (var link in links)
            {
                var hasLinkEntry = hasLinkEntries.FirstOrDefault(x => x.Id.Equals(link.EntryId));
                if (hasLinkEntry != null)
                {
                    hasLinkEntry.Links.Remove(link);
                }
            }
            if (!hasLinkEntries.IsNullOrEmpty())
            {
                await SaveListCatalogEntitiesAsync(hasLinkEntries.ToArray());
            }

            return NoContent();
        }

        /// <summary>
        /// Move categories or products to another location.
        /// </summary>
        /// <param name="moveInfo">Move operation details</param>
        [HttpPost]
        [Route("move")]
        public async Task<ActionResult> Move([FromBody]MoveInfo moveInfo)
        {
            var dstCatalog = (await _catalogService.GetByIdsAsync(new[] { moveInfo.Catalog })).FirstOrDefault();
            if (dstCatalog.IsVirtual)
            {
                return BadRequest("Unable to move to an virtual catalog");
            }
            var catalogEntries = (await LoadCatalogEntriesAsync<IEntity>(moveInfo.ListEntries.Select(x => x.Id).ToArray())).ToList();
            foreach (var listEntry in moveInfo.ListEntries.ToList())
            {
                var existEntry = catalogEntries.FirstOrDefault(x => x.Equals(listEntry));
                if (existEntry != null)
                {
                    if (existEntry is Category category)
                    {
                        category.Move(moveInfo.Catalog, moveInfo.Category);
                    }
                    if (existEntry is CatalogProduct product)
                    {
                        product.Move(moveInfo.Catalog, moveInfo.Category);
                        if (!product.Variations.IsNullOrEmpty())
                        {
                            catalogEntries.AddRange(product.Variations);
                        }
                    }
                }
            }

            //TODO:
            //Scope bound security check
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Create, categories);
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Create, products);
            await SaveListCatalogEntitiesAsync(catalogEntries.ToArray());
            return NoContent();
        }

        private async Task SaveListCatalogEntitiesAsync(IEntity[] entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                var products = entities.OfType<CatalogProduct>().ToArray();
                if (!products.IsNullOrEmpty())
                {
                    await _itemService.SaveChangesAsync(products);
                }
                var categories = entities.OfType<Category>().ToArray();
                if (!categories.IsNullOrEmpty())
                {
                    await _categoryService.SaveChangesAsync(categories);
                }
            }
        }

        private async Task<IList<T>> LoadCatalogEntriesAsync<T>(string[] ids)
        {
            var products = await _itemService.GetByIdsAsync(ids, (ItemResponseGroup.Links | ItemResponseGroup.Variations).ToString());
            var categories = await _categoryService.GetByIdsAsync(ids.Except(products.Select(x => x.Id)).ToArray(), CategoryResponseGroup.WithLinks.ToString());
            return products.OfType<T>().Concat(categories.OfType<T>()).ToList();
        }
    }
}
