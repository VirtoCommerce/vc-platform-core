using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.MarketingModule.Core;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions.Search;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.MarketingModule.Data.Promotions;
using VirtoCommerce.MarketingModule.Data.Repositories;
using VirtoCommerce.MarketingModule.Web.Converters;
using VirtoCommerce.MarketingModule.Web.ExportImport;
using VirtoCommerce.MarketingModule.Web.Model;
using VirtoCommerce.MarketingModule.Web.Model.PushNotifications;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.PushNotifications;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.MarketingModule.Web.Controllers.Api
{
    [Route("api/marketing/promotions")]
    public class MarketingModulePromotionController : Controller
    {
        private readonly IPromotionService _promotionService;
        private readonly ICouponService _couponService;
        private readonly IMarketingPromoEvaluator _promoEvaluator;
        private readonly IPromotionSearchService _promoSearchService;
        private readonly IUserNameResolver _userNameResolver;
        private readonly IPushNotificationManager _notifier;
        private readonly IBlobStorageProvider _blobStorageProvider;
        private readonly CsvCouponImporter _csvCouponImporter;
        private readonly Func<IMarketingRepository> _repositoryFactory;

        public MarketingModulePromotionController(
            IPromotionService promotionService,
            ICouponService couponService,
            IMarketingPromoEvaluator promoEvaluator,
            IPromotionSearchService promoSearchService,
            IUserNameResolver userNameResolver,
            IPushNotificationManager notifier,
            IBlobStorageProvider blobStorageProvider,
            CsvCouponImporter csvCouponImporter,
            Func<IMarketingRepository> repositoryFactory
            )
        {
            _promotionService = promotionService;
            _couponService = couponService;
            _promoEvaluator = promoEvaluator;
            _promoSearchService = promoSearchService;
            _userNameResolver = userNameResolver;
            _notifier = notifier;
            _blobStorageProvider = blobStorageProvider;
            _csvCouponImporter = csvCouponImporter;
            _repositoryFactory = repositoryFactory;
        }

        /// <summary>
        /// Search dynamic content places by given criteria
        /// </summary>
        /// <param name="criteria">criteria</param>
        [HttpPost]
        [Route("search")]
        public async Task<ActionResult<GenericSearchResult<DynamicPromotion>>> PromotionsSearch([FromBody]PromotionSearchCriteria criteria)
        {
            var retVal = new GenericSearchResult<Promotion>();
            //Scope bound ACL filtration
            criteria = FilterPromotionSearchCriteria(User.Identity.Name, criteria);

            var promoSearchResult = await _promoSearchService.SearchPromotionsAsync(criteria);
            //foreach (var promotion in promoSearchResult.Results.OfType<DynamicPromotion>())
            //{
            //    promotion.PredicateVisualTreeSerialized = null;
            //    promotion.PredicateSerialized = null;
            //    promotion.RewardsSerialized = null;
            //}

            retVal.TotalCount = promoSearchResult.TotalCount;
            retVal.Results = promoSearchResult.Results.ToList();
            return Ok(retVal);
        }

        /// <summary>
        /// Evaluate promotions
        /// </summary>
        /// <param name="context">Promotion evaluation context</param>
        [HttpPost]
        [Route("evaluate")]
        public async Task<ActionResult<Model.PromotionReward[]>> EvaluatePromotions([FromBody]PromotionEvaluationContext context)
        {
            var retVal = await _promoEvaluator.EvaluatePromotionAsync(context);
            return Ok(retVal.Rewards.Select(x => x.ToWebModel()).ToArray());
        }

        /// <summary>
        /// Find promotion object by id
        /// </summary>
        /// <remarks>Return a single promotion (dynamic or custom) object </remarks>
        /// <param name="id">promotion id</param>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Promotion>> GetPromotionById(string id)
        {
            var promotions = await _promotionService.GetPromotionsByIdsAsync(new[] { id });
            var retVal = promotions.FirstOrDefault();
            if (retVal != null)
            {
                //TODO
                //var scopes = _permissionScopeService.GetObjectPermissionScopeStrings(retVal).ToArray();
                //if (!_securityService.UserHasAnyPermission(User.Identity.Name, scopes, MarketingPredefinedPermissions.Read))
                //{
                //    throw new HttpResponseException(HttpStatusCode.Unauthorized);
                //}

                return Ok(retVal);
            }
            return NotFound();
        }

        /// <summary>
        /// Get new dynamic promotion object 
        /// </summary>
        /// <remarks>Return a new dynamic promotion object with populated dynamic expression tree</remarks>
        [HttpGet]
        [Route("new")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public ActionResult<Promotion> GetNewDynamicPromotion()
        {
            var retVal = AbstractTypeFactory<DynamicPromotion>.TryCreateInstance();
            retVal.IsActive = true;
            return Ok(retVal);
        }

        /// <summary>
        /// Add new dynamic promotion object to marketing system
        /// </summary>
        /// <param name="promotion">dynamic promotion object that needs to be added to the marketing system</param>
        [HttpPost]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<ActionResult<Promotion>> CreatePromotion([FromBody]Promotion promotion)
        {
            //TODO
            //var scopes = _permissionScopeService.GetObjectPermissionScopeStrings(promotion).ToArray();
            //if (!_securityService.UserHasAnyPermission(User.Identity.Name, scopes, MarketingPredefinedPermissions.Create))
            //{
            //    throw new HttpResponseException(HttpStatusCode.Unauthorized);
            //}
            await _promotionService.SavePromotionsAsync(new[] { promotion });
            return await GetPromotionById(promotion.Id);
        }


        /// <summary>
        /// Update a existing dynamic promotion object in marketing system
        /// </summary>
        /// <param name="promotion">>dynamic promotion object that needs to be updated in the marketing system</param>
        [HttpPut]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<IActionResult> UpdatePromotions([FromBody]Promotion promotion)
        {
            //TODO
            //var scopes = _permissionScopeService.GetObjectPermissionScopeStrings(promotion).ToArray();
            //if (!_securityService.UserHasAnyPermission(User.Identity.Name, scopes, MarketingPredefinedPermissions.Update))
            //{
            //    throw new HttpResponseException(HttpStatusCode.Unauthorized);
            //}
            await _promotionService.SavePromotionsAsync(new[] { promotion });
            return Ok();
        }

        /// <summary>
        ///  Delete promotions objects
        /// </summary>
        /// <param name="ids">promotions object ids for delete in the marketing system</param>
        [HttpDelete]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.Delete)]
        public async Task<IActionResult> DeletePromotions([FromQuery] string[] ids)
        {
            await _promotionService.DeletePromotionsAsync(ids);
            return Ok();
        }

        [HttpPost]
        [Route("coupons/search")]
        public async Task<ActionResult<GenericSearchResult<Coupon>>> SearchCoupons([FromBody]CouponSearchCriteria criteria)
        {
            var searchResult = await _couponService.SearchCouponsAsync(criteria);
            // actualize coupon totalUsage field 
            using (var repository = _repositoryFactory())
            {
                var ids = searchResult.Results.Select(x => x.Id).ToArray();
                var couponEntities = await repository.GetCouponsByIdsAsync(ids);
                foreach (var coupon in searchResult.Results)
                {
                    coupon.TotalUsesCount = couponEntities.First(c => c.Id == coupon.Id).TotalUsesCount;
                }
            }
            return Ok(searchResult);
        }

        [HttpGet]
        [Route("coupons/{id}")]
        public async Task<ActionResult<Coupon>> GetCoupon(string id)
        {
            var coupons = await _couponService.GetByIdsAsync(new[] { id });
            var coupon = coupons.FirstOrDefault();

            return Ok(coupon);
        }

        [HttpPost]
        [Route("coupons/add")]
        public async Task<IActionResult> AddCoupons([FromBody]Coupon[] coupons)
        {
            await _couponService.SaveCouponsAsync(coupons);

            return Ok();
        }

        [HttpDelete]
        [Route("coupons/delete")]
        public async Task<IActionResult> DeleteCoupons([FromQuery] string[] ids)
        {
            await _couponService.DeleteCouponsAsync(ids);

            return Ok();
        }

        [HttpPost]
        [Route("coupons/import")]
        public ActionResult<ImportNotification> ImportCoupons([FromBody]ImportRequest request)
        {
            var notification = new ImportNotification(_userNameResolver.GetCurrentUserName())
            {
                Title = "Import coupons from CSV",
                Description = "Starting import..."
            };

            //TODO
            //_notifier.Upsert(notification);

            //BackgroundJob.Enqueue(() => BackgroundImport(request, notification));

            return Ok(notification);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public void BackgroundImport(ImportRequest request, ImportNotification notification)
        {
            //TODO
            //Action<ExportImportProgressInfo> progressCallback = c =>
            //{
            //    notification.InjectFrom(c);
            //    _notifier.Upsert(notification);
            //};

            //using (var stream = _blobStorageProvider.OpenRead(request.FileUrl))
            //{
            //    try
            //    {
            //        _csvCouponImporter.DoImport(stream, request.Delimiter, request.PromotionId, request.ExpirationDate, progressCallback);
            //    }
            //    catch (Exception exception)
            //    {
            //        notification.Description = "Import error";
            //        notification.ErrorCount++;
            //        notification.Errors.Add(exception.ToString());
            //    }
            //    finally
            //    {
            //        notification.Finished = DateTime.UtcNow;
            //        notification.Description = "Import finished" + (notification.Errors.Any() ? " with errors" : " successfully");
            //        _notifier.Upsert(notification);
            //    }
            //}
        }


        private PromotionSearchCriteria FilterPromotionSearchCriteria(string userName, PromotionSearchCriteria criteria)
        {
            //TODO
            //if (!_securityService.UserHasAnyPermission(userName, null, MarketingPredefinedPermissions.Read))
            //{
            //    //Get defined user 'read' permission scopes
            //    var readPermissionScopes = _securityService.GetUserPermissions(userName)
            //                                          .Where(x => x.Id.StartsWith(MarketingPredefinedPermissions.Read))
            //                                          .SelectMany(x => x.AssignedScopes)
            //                                          .ToList();

            //    //Check user has a scopes
            //    //Store
            //    criteria.Store = readPermissionScopes.OfType<MarketingSelectedStoreScope>()
            //                                             .Select(x => x.Scope)
            //                                             .Where(x => !String.IsNullOrEmpty(x))
            //                                             .FirstOrDefault();
            //}
            return criteria;
        }
    }
}
