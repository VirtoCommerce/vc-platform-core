using System;
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
        [Route("menu")]
        [Authorize(Permissions.Read)]
        public async Task<ActionResult<MenuLinkList[]>> GetListsAsync([FromRoute]string storeId)
        {
            var lists = (await _menuService.GetListsByStoreIdAsync(storeId)).ToArray();

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
        [Route("menu/{listId}")]
        [Authorize(Permissions.Read)]
        public async Task<ActionResult<MenuLinkList>> GetListAsync([FromRoute]string storeId, [FromRoute]string listId)
        {
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
        [Route("menu/checkname")]
        [Authorize(Permissions.Read)]
        public async Task<ActionResult<bool>> CheckNameAsync([FromRoute]string storeId, [FromQuery]string name, [FromQuery]string language = "", [FromQuery]string id = "")
        {
            var retVal = await _menuService.CheckListAsync(storeId, name, language, id);
            return Ok(new { Result = retVal });
        }

        /// <summary>
        /// Update menu link list
        /// </summary>
        /// <param name="list">Menu link list</param>
        [HttpPost]
        [Route("menu")]
        [Authorize(Permissions.Update)]
        public async Task<ActionResult> UpdateMenuLinkList([FromBody]MenuLinkList list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            await _menuService.AddOrUpdateAsync(list);
            return NoContent();
        }

        /// <summary>
        /// Delete menu link list
        /// </summary>
        /// <param name="listIds">Menu link list id</param>
        [HttpDelete]
        [Route("menu")]
        [Authorize(Permissions.Delete)]
        public async Task<ActionResult> DeleteMenuLinkLists([FromQuery] string[] listIds)
        {
            if (listIds == null)
                throw new ArgumentNullException(nameof(listIds));

            foreach (var listId in listIds)
            {
                var list = await _menuService.GetListByIdAsync(listId);
            }
            await _menuService.DeleteListsAsync(listIds);

            return NoContent();
        }

    }
}
