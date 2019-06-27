using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.StoreModule.Core;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Model.Search;
using VirtoCommerce.StoreModule.Core.Notifications;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.StoreModule.Web.Model;

namespace VirtoCommerce.StoreModule.Web.Controllers.Api
{
    [Route("api/stores")]
    public class StoreModuleController : Controller
    {
        private readonly IStoreService _storeService;
        private readonly IStoreSearchService _storeSearchService;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly INotificationSender _notificationSender;
        //private readonly IPermissionScopeService _permissionScopeService;

        public StoreModuleController(IStoreService storeService, IStoreSearchService storeSearchService, UserManager<ApplicationUser> userManager
            , INotificationSender notificationSender//, IPermissionScopeService permissionScopeService
            )
        {
            _storeService = storeService;
            _storeSearchService = storeSearchService;
            _userManager = userManager;
            _notificationSender = notificationSender;
            //_permissionScopeService = permissionScopeService;
        }

        /// <summary>
        /// Search stores
        /// </summary>
        [HttpPost]
        [Route("search")]
        public async Task<StoreSearchResult> SearchStores([FromBody]StoreSearchCriteria criteria)
        {
            //Filter resulting stores correspond to current user permissions
            //first check global permission
            //TODO
            //if (!_securityService.UserHasAnyPermission(User.Identity.Name, null, StorePredefinedPermissions.Read))
            //{
            //    //Get user 'read' permission scopes
            //    criteria.StoreIds = _securityService.GetUserPermissions(User.Identity.Name)
            //                                          .Where(x => x.Id.StartsWith(StorePredefinedPermissions.Read))
            //                                          .SelectMany(x => x.AssignedScopes)
            //                                          .OfType<StoreSelectedScope>()
            //                                          .Select(x => x.Scope)
            //                                          .ToArray();
            //    //Do not return all stores if user don't have corresponding permission
            //    if (criteria.StoreIds.IsNullOrEmpty())
            //    {
            //        throw new HttpResponseException(HttpStatusCode.Unauthorized);
            //    }
            //}
            criteria.ResponseGroup = StoreResponseGroup.StoreInfo.ToString();
            var result = await _storeSearchService.SearchStoresAsync(criteria);
            return result;
        }

        /// <summary>
        /// Get all stores
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<Store[]>> GetStores()
        {
            var criteria = new StoreSearchCriteria
            {
                Skip = 0,
                Take = int.MaxValue
            };
            var storesSearchResult = await SearchStores(criteria);
            return Ok(storesSearchResult.Results);
        }

        /// <summary>
        /// Get store by id
        /// </summary>
        /// <param name="id">Store id</param>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Store>> GetStoreById(string id)
        {
            var result = await _storeService.GetByIdAsync(id, StoreResponseGroup.Full.ToString());
            //TODO
            //CheckCurrentUserHasPermissionForObjects(StorePredefinedPermissions.Read, result);
            //result.Scopes = _permissionScopeService.GetObjectPermissionScopeStrings(result).ToArray();
            return Ok(result);
        }


        /// <summary>
        /// Create store
        /// </summary>
        /// <param name="store">Store</param>
        [HttpPost]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<ActionResult<Store>> CreateStore([FromBody]Store store)
        {
            await _storeService.SaveChangesAsync(new[] { store });
            return Ok(store);
        }

        /// <summary>
        /// Update store
        /// </summary>
        /// <param name="store">Store</param>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult> UpdateStore([FromBody]Store store)
        {
            //TODO
            //CheckCurrentUserHasPermissionForObjects(StorePredefinedPermissions.Update, store);
            await _storeService.SaveChangesAsync(new[] { store });
            return NoContent();
        }

        /// <summary>
        /// Delete stores
        /// </summary>
        /// <param name="ids">Ids of store that needed to delete</param>
        [HttpDelete]
        [Route("")]
        public async Task<ActionResult> DeleteStore([FromQuery] string[] ids)
        {
            //TODO
            //CheckCurrentUserHasPermissionForObjects(StorePredefinedPermissions.Delete, stores);
            await _storeService.DeleteAsync(ids);
            return NoContent();
        }

        /// <summary>
        /// Send dynamic notification (contains custom list of properties) to store or administrator email 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("send/dynamicnotification")]
        public async Task<ActionResult> SendDynamicNotificationAnStoreEmail(SendDynamicNotificationRequest request)
        {
            var store = await _storeService.GetByIdAsync(request.StoreId, StoreResponseGroup.StoreInfo.ToString());

            if (store == null)
                throw new InvalidOperationException(string.Concat("Store not found. StoreId: ", request.StoreId));

            if (string.IsNullOrEmpty(store.Email) && string.IsNullOrEmpty(store.AdminEmail))
                throw new InvalidOperationException(string.Concat("Both store email and admin email are empty. StoreId: ", request.StoreId));

            var notification = new StoreDynamicEmailNotification()
            {
                FormType = request.Type,
                Fields = request.Fields
            };
            await _notificationSender.SendNotificationAsync(notification, request.Language);

            return NoContent();
        }

        /// <summary>
        /// Check if given contact has login on behalf permission
        /// </summary>
        /// <param name="storeId">Store ID</param>
        /// <param name="id">Contact ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{storeId}/accounts/{id}/loginonbehalf")]
        public async Task<ActionResult<LoginOnBehalfInfo>> GetLoginOnBehalfInfo(string storeId, string id)
        {
            var result = new LoginOnBehalfInfo
            {
                UserName = id
            };

            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                //TODO
                //result.CanLoginOnBehalf = _securityService.UserHasAnyPermission(user.UserName, null, StorePredefinedPermissions.LoginOnBehalf);
            }

            return Ok(result);
        }

        /// <summary>
        /// Returns list of stores which user can sign in
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("allowed/{userId}")]
        public async Task<ActionResult<Store[]>> GetUserAllowedStores(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var storeIds = await _storeService.GetUserAllowedStoreIdsAsync(user);
                var stores = await _storeService.GetByIdsAsync(storeIds.ToArray(), StoreResponseGroup.StoreInfo.ToString());
                return Ok(stores);
            }

            return NotFound();
        }

        //TODO
        //private void CheckCurrentUserHasPermissionForObjects(string permission, params coreModel.Store[] objects)
        //{
        //    //Scope bound security check
        //    var scopes = objects.SelectMany(x => _permissionScopeService.GetObjectPermissionScopeStrings(x)).Distinct().ToArray();
        //    if (!_securityService.UserHasAnyPermission(User.Identity.Name, scopes, permission))
        //    {
        //        throw new HttpResponseException(HttpStatusCode.Unauthorized);
        //    }
        //}
    }
}
