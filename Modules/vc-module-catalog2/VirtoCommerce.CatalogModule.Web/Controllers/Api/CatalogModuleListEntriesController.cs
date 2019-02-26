using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.CatalogModule.Core2.Exceptions;
using VirtoCommerce.CatalogModule.Core2.Model;
using VirtoCommerce.CatalogModule.Core2.Model.ListEntry;
using VirtoCommerce.CatalogModule.Core2.Model.Search;
using VirtoCommerce.CatalogModule.Core2.Services;
using VirtoCommerce.CatalogModule.Web2.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Web2.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/catalog/listentries")]
    public class CatalogModuleListEntriesController : Controller
    {
        private readonly IListEntrySearchService _listEntrySearchService;
        private readonly ICategoryService _categoryService;
        private readonly IItemService _itemService;
        private readonly ICatalogService _catalogService;

        public CatalogModuleListEntriesController(IListEntrySearchService listEntrySearchService, ICategoryService categoryService, IItemService itemService, ICatalogService catalogService)            
        {
            _catalogService = catalogService;
            _listEntrySearchService = listEntrySearchService;
            _categoryService = categoryService;
            _itemService = itemService;
        }

        /// <summary>
        /// Searches for the items by complex criteria.
        /// </summary>
        /// <param name="criteria">The search criteria.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(GenericSearchResult<ListEntryBase>), 200)]
        public ActionResult ListItemsSearch([FromBody] CatalogListEntrySearchCriteria criteria)
        {
            //TODO:
            //ApplyRestrictionsForCurrentUser(coreModelCriteria);
            var result = _listEntrySearchService.Search(criteria);
            return Ok(result);
        }

        /// <summary>
        /// Creates links for categories or items to parent categories and catalogs.
        /// </summary>
        /// <param name="links">The links.</param>
        [HttpPost]
        [Route("~/api/catalog/listentrylinks")]
        [ProducesResponseType(200)]
        public ActionResult CreateLinks([FromBody] CategoryLink[] links)
        {
            //TODO:
            //Scope bound security check
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Create, links);
            var entryIds = links.Select(x => x.EntryId);
            var hasLinkEntries = LoadCatalogEntries<IHasLinks>(entryIds);
            foreach (var link in links)
            {
                var hasLinkEntry = hasLinkEntries.FirstOrDefault(x => x.Id.Equals(link.EntryId));
                if(hasLinkEntry != null && !hasLinkEntry.Links.Contains(link))
                {
                    hasLinkEntry.Links.Add(link);
                }
            }
            _itemService.SaveChanges(hasLinkEntries.OfType<CatalogProduct>());
            _categoryService.SaveChanges(hasLinkEntries.OfType<Category>());
            return Ok();
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        [Route("~/api/catalog/getslug")]
        [ProducesResponseType(typeof(string), 200)]
        public ActionResult GetSlug(string text)
        {         
            return Ok(text.GenerateSlug());
        }


        /// <summary>
        /// Unlinks the linked categories or items from parent categories and catalogs.
        /// </summary>
        /// <param name="links">The links.</param>
        [HttpPost]
        [Route("~/api/catalog/listentrylinks/delete")]
        [ProducesResponseType(200)]
        public ActionResult DeleteLinks(CategoryLink[] links)
        {
            //TODO:
            //Scope bound security check
            //CheckCurrentUserHasPermissionForObjects(CatalogPredefinedPermissions.Delete, links);
            var entryIds = links.Select(x => x.EntryId);
            var hasLinkEntries = LoadCatalogEntries<IHasLinks>(entryIds);
            foreach (var link in links)
            {
                var hasLinkEntry = hasLinkEntries.FirstOrDefault(x => x.Id.Equals(link.EntryId));
                if (hasLinkEntry != null)
                {
                    hasLinkEntry.Links.Remove(link);
                }
            }
            _itemService.SaveChanges(hasLinkEntries.OfType<CatalogProduct>());
            _categoryService.SaveChanges(hasLinkEntries.OfType<Category>());

            return Ok();
        }


        /// <summary>
        /// Move categories or products to another location.
        /// </summary>
        /// <param name="moveInfo">Move operation details</param>
        [HttpPost]
        [Route("move")]
        [ProducesResponseType(200)]
        public ActionResult Move([FromBody]MoveInfo moveInfo)
        {
            var dstCatalog = _catalogService.GetByIds(new[] { moveInfo.Catalog }).FirstOrDefault();
            if (dstCatalog.IsVirtual)
            {
                throw new CatalogModuleException("Unable to move to an virtual catalog");
            }
            var catalogEntries = LoadCatalogEntries<IEntity>(moveInfo.ListEntries.Select(x => x.Id)).ToList();
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
                        if(!product.Variations.IsNullOrEmpty())
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

            _itemService.SaveChanges(catalogEntries.OfType<CatalogProduct>());
            _categoryService.SaveChanges(catalogEntries.OfType<Category>());
            return Ok();
        }

        private IEnumerable<T> LoadCatalogEntries<T>(IEnumerable<string> ids)
        {
            var result = _categoryService.GetByIds(ids, (ItemResponseGroup.Links | ItemResponseGroup.Variations).ToString()).OfType<T>()
                                                 .Concat(_itemService.GetByIds(ids, CategoryResponseGroup.WithLinks.ToString()).OfType<T>());
            return result;
        }


    }
}
