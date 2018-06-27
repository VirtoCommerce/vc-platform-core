using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.ContentModule.Core.Services;

namespace VirtoCommerce.ContentModule.Web.Controllers.Api
{
    [RoutePrefix("api/cms/{storeId}")]
    public class MenuController : ContentBaseController
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService, ISecurityService securityService, IPermissionScopeService permissionScopeService)
            : base(securityService, permissionScopeService)
        {
            if (menuService == null)
                throw new ArgumentNullException(nameof(menuService));

            _menuService = menuService;
        }

        /// <summary>
        /// Get menu link lists
        /// </summary>
        /// <param name="storeId">Store id</param>
		[HttpGet]
        [ResponseType(typeof(IEnumerable<MenuLinkList>))]
        [Route("menu")]
        public IHttpActionResult GetLists(string storeId)
        {
            CheckCurrentUserHasPermissionForObjects(ContentPredefinedPermissions.Read, new ContentScopeObject { StoreId = storeId });

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
        [ResponseType(typeof(MenuLinkList))]
        [Route("menu/{listId}")]
        public IHttpActionResult GetList(string storeId, string listId)
        {
            CheckCurrentUserHasPermissionForObjects(ContentPredefinedPermissions.Read, new ContentScopeObject { StoreId = storeId });

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
        [ResponseType(typeof(bool))]
        [Route("menu/checkname")]
        public IHttpActionResult CheckName(string storeId, string name, string language = "", string id = "")
        {
            CheckCurrentUserHasPermissionForObjects(ContentPredefinedPermissions.Read, new ContentScopeObject { StoreId = storeId });

            var retVal = _menuService.CheckList(storeId, name, language, id);
            return Ok(new { Result = retVal });
        }

        /// <summary>
        /// Update menu link list
        /// </summary>
        /// <param name="list">Menu link list</param>
        [HttpPost]
        [ResponseType(typeof(void))]
        [Route("menu")]
        public IHttpActionResult Update(MenuLinkList list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            CheckCurrentUserHasPermissionForObjects(ContentPredefinedPermissions.Update, new ContentScopeObject { StoreId = list.StoreId });

            _menuService.AddOrUpdate(list.ToCoreModel());
            return Ok();
        }

        /// <summary>
        /// Delete menu link list
        /// </summary>
        /// <param name="listIds">Menu link list id</param>
        [HttpDelete]
        [ResponseType(typeof(void))]
        [Route("menu")]
        public IHttpActionResult Delete([FromUri] string[] listIds)
        {
            if (listIds == null)
                throw new ArgumentNullException(nameof(listIds));

            foreach (var listId in listIds)
            {
                var list = _menuService.GetListById(listId).ToWebModel();
                CheckCurrentUserHasPermissionForObjects(ContentPredefinedPermissions.Delete, new ContentScopeObject { StoreId = list.StoreId });
            }
            _menuService.DeleteLists(listIds);
            return Ok();
        }

    }
}
