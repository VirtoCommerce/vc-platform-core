using System;
using System.Collections.Generic;
using System.Linq;
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
        public IActionResult GetLists(string storeId)
        {
            //ToDo
            //CheckCurrentUserHasPermissionForObjects(ContentPredefinedPermissions.Read, new ContentScopeObject { StoreId = storeId });

            var lists = _menuService.GetListsByStoreId(storeId).ToList();
            if (lists.Any())
            {
                return Ok(lists.Select(s => s.ToWebModel()));
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
        public IActionResult GetList(string storeId, string listId)
        {
            //ToDo
            //CheckCurrentUserHasPermissionForObjects(ContentPredefinedPermissions.Read, new ContentScopeObject { StoreId = storeId });

            var item = _menuService.GetListById(listId).ToWebModel();
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
        public IActionResult CheckName(string storeId, string name, string language = "", string id = "")
        {
            //ToDo
            //CheckCurrentUserHasPermissionForObjects(ContentPredefinedPermissions.Read, new ContentScopeObject { StoreId = storeId });

            var retVal = _menuService.CheckListAsync(storeId, name, language, id);
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
        public IActionResult Update(MenuLinkList list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            //ToDo
            //CheckCurrentUserHasPermissionForObjects(ContentPredefinedPermissions.Update, new ContentScopeObject { StoreId = list.StoreId });

            _menuService.AddOrUpdate(list.ToCoreModel());
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
        public IActionResult Delete([FromQuery] string[] listIds)
        {
            if (listIds == null)
                throw new ArgumentNullException(nameof(listIds));

            foreach (var listId in listIds)
            {
                var list = _menuService.GetListById(listId).ToWebModel();
                //ToDo
                //CheckCurrentUserHasPermissionForObjects(ContentPredefinedPermissions.Delete, new ContentScopeObject { StoreId = list.StoreId });
            }
            _menuService.DeleteLists(listIds);
            return Ok();
        }

    }
}
