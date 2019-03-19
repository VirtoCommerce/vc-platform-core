using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;

using Permissions = VirtoCommerce.ContentModule.Core.ContentConstants.Security.Permissions;

namespace VirtoCommerce.ContentModule.Web.Controllers.Api
{
    [Route("api/cms/{storeId}")]
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        /// <summary>
        /// Get menu link lists
        /// </summary>
        /// <param name="storeId">Store id</param>
		[HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MenuLinkList>), 200)]
        [Route("menu")]
        [Authorize(Permissions.Read)]
        public async Task<IActionResult> GetListsAsync([FromRoute]string storeId)
        {
            //ToDo
            //CheckCurrentUserHasPermissionForObjects(ContentPredefinedPermissions.Read, new ContentScopeObject { StoreId = storeId });

            var lists = await _menuService.GetListsByStoreIdAsync(storeId);

            if (lists.Any())
            {
                return Ok(lists);
            }
            return Ok();
        }

        /// <summary>
        /// Get menu link list by id
        /// </summary>
        /// <param name="listId">List id</param>
        /// <param name="storeId">Store id</param>
        [HttpGet]
        [ProducesResponseType(typeof(MenuLinkList), 200)]
        [Route("menu/{listId}")]
        [Authorize(Permissions.Read)]
        public async Task<IActionResult> GetListAsync([FromRoute]string storeId, [FromRoute]string listId)
        {
            //ToDo
            //CheckCurrentUserHasPermissionForObjects(ContentPredefinedPermissions.Read, new ContentScopeObject { StoreId = storeId });

            var item = await _menuService.GetListByIdAsync(listId);
            return Ok(item);
        }

        /// <summary>
        /// Checking name of menu link list
        /// </summary>
        /// <remarks>Checking pair of name+language of menu link list for unique, if checking result - false saving unavailable</remarks>
        /// <param name="storeId">Store id</param>
        /// <param name="name">Name of menu link list</param>
        /// <param name="language">Language of menu link list</param>
        /// <param name="id">Menu link list id</param>
        [HttpGet]
        [ProducesResponseType(typeof(bool), 200)]
        [Route("menu/checkname")]
        [Authorize(Permissions.Read)]
        public async Task<IActionResult> CheckNameAsync([FromRoute]string storeId, [FromQuery]string name, [FromQuery]string language = "", [FromQuery]string id = "")
        {
            //ToDo
            //CheckCurrentUserHasPermissionForObjects(ContentPredefinedPermissions.Read, new ContentScopeObject { StoreId = storeId });

            var retVal = await _menuService.CheckListAsync(storeId, name, language, id);
            return Ok(new { Result = retVal });
        }

        /// <summary>
        /// Update menu link list
        /// </summary>
        /// <param name="list">Menu link list</param>
        [HttpPost]
        [ProducesResponseType(200)]
        [Route("menu")]
        [Authorize(Permissions.Update)]
        public async Task<IActionResult> UpdateAsync([FromBody]MenuLinkList list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            //ToDo
            //CheckCurrentUserHasPermissionForObjects(ContentPredefinedPermissions.Update, new ContentScopeObject { StoreId = list.StoreId });

            await _menuService.AddOrUpdateAsync(list);
            return Ok();
        }

        /// <summary>
        /// Delete menu link list
        /// </summary>
        /// <param name="listIds">Menu link list id</param>
        [HttpDelete]
        [ProducesResponseType(200)]
        [Route("menu")]
        [Authorize(Permissions.Delete)]
        public async Task<IActionResult> DeleteAsync([FromQuery] string[] listIds)
        {
            if (listIds == null)
                throw new ArgumentNullException(nameof(listIds));

            foreach (var listId in listIds)
            {
                var list = await _menuService.GetListByIdAsync(listId);

                //ToDo
                //CheckCurrentUserHasPermissionForObjects(ContentPredefinedPermissions.Delete, new ContentScopeObject { StoreId = list.StoreId });
            }
            await _menuService.DeleteListsAsync(listIds);

            return Ok();
        }

    }
}
