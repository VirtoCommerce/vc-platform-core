using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.SubscriptionModule.Core;
using VirtoCommerce.SubscriptionModule.Core.Model;
using VirtoCommerce.SubscriptionModule.Core.Model.Search;
using VirtoCommerce.SubscriptionModule.Core.Services;
using VirtoCommerce.SubscriptionModule.Web.Model;

namespace VirtoCommerce.SubscriptionModule.Web.Controllers.Api
{
    [Route("api/subscriptions")]
    public class SubscriptionModuleController : Controller
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ISubscriptionSearchService _subscriptionSearchService;
        private readonly IPaymentPlanService _planService;
        private readonly ISubscriptionBuilder _subscriptionBuilder;
        private readonly ICustomerOrderService _customerOrderService;
        public SubscriptionModuleController(ISubscriptionService subscriptionService, ISubscriptionSearchService subscriptionSearchService, IPaymentPlanService planService,
                                            ISubscriptionBuilder subscriptionBuilder, ICustomerOrderService customerOrderService)
        {
            _subscriptionService = subscriptionService;
            _subscriptionSearchService = subscriptionSearchService;
            _planService = planService;
            _subscriptionBuilder = subscriptionBuilder;
            _customerOrderService = customerOrderService;
        }

        /// <summary>
        /// Search subscriptions by given criteria
        /// </summary>
        /// <param name="criteria">criteria</param>
        [HttpPost]
        [Route("search")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<SubscriptionSearchResult>> SearchSubscriptions([FromBody] SubscriptionSearchCriteria criteria)
        {
            var result = await _subscriptionSearchService.SearchSubscriptionsAsync(criteria);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(Subscription), 200)]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<IActionResult> GetSubscriptionById(string id, [FromQuery] string respGroup = null)
        {
            var retVal = (await _subscriptionService.GetByIdsAsync(new[] { id }, respGroup)).FirstOrDefault();
            if (retVal != null)
            {
                retVal = (await _subscriptionBuilder.TakeSubscription(retVal).ActualizeAsync()).Subscription;
            }
            return Ok(retVal);
        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(Subscription[]), 200)]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<IActionResult> GetSubscriptionByIds([FromQuery] string[] ids, [FromQuery] string respGroup = null)
        {
            var retVal = await _subscriptionService.GetByIdsAsync(ids, respGroup);
            foreach (var subscription in retVal)
            {
                await _subscriptionBuilder.TakeSubscription(subscription).ActualizeAsync();
            }
            return Ok(retVal);
        }

        [HttpPost]
        [Route("order")]
        [ProducesResponseType(typeof(CustomerOrder), 200)]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<IActionResult> CreateReccurentOrderForSubscription([FromBody] Subscription subscription)
        {
            var subscriptionBuilder = await _subscriptionBuilder.TakeSubscription(subscription).ActualizeAsync();
            var order = await subscriptionBuilder.TryToCreateRecurrentOrderAsync(forceCreation: true);
            await _customerOrderService.SaveChangesAsync(new[] { order });
            return Ok(order);
        }

        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(Subscription), 200)]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<IActionResult> CreateSubscription([FromBody] Subscription subscription)
        {
            await _subscriptionBuilder.TakeSubscription(subscription).ActualizeAsync();
            await _subscriptionService.SaveSubscriptionsAsync(new[] { subscription });
            return Ok(subscription);
        }

        [HttpPost]
        [Route("cancel")]
        [ProducesResponseType(typeof(Subscription), 200)]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<IActionResult> CancelSubscription([FromBody] SubscriptionCancelRequest cancelRequest)
        {
            var retVal = (await _subscriptionService.GetByIdsAsync(new[] { cancelRequest.SubscriptionId })).FirstOrDefault();
            if (retVal != null)
            {
                await _subscriptionBuilder.TakeSubscription(retVal).CancelSubscription(cancelRequest.CancelReason).ActualizeAsync();
                await _subscriptionService.SaveSubscriptionsAsync(new[] { retVal });
            }
            return Ok(retVal);
        }

        [HttpPut]
        [Route("")]
        [ProducesResponseType(typeof(Subscription), 200)]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<IActionResult> UpdateSubscription([FromBody] Subscription subscription)
        {
            await _subscriptionBuilder.TakeSubscription(subscription).ActualizeAsync();
            await _subscriptionService.SaveSubscriptionsAsync(new[] { subscription });
            return Ok(subscription);
        }

        /// <summary>
        ///  Delete subscriptions
        /// </summary>
        /// <param name="ids">subscriptions' ids for delete</param>
        [HttpDelete]
        [Route("")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        [Authorize(ModuleConstants.Security.Permissions.Delete)]
        public async Task<IActionResult> DeleteSubscriptionsByIds([FromQuery] string[] ids)
        {
            await _subscriptionService.DeleteAsync(ids);
            return NoContent();
        }


        [HttpGet]
        [Route("plans/{id}")]
        [ProducesResponseType(typeof(PaymentPlan), 200)]
        public async Task<IActionResult> GetPaymentPlanById(string id)
        {
            var retVal = (await _planService.GetByIdsAsync(new[] { id })).FirstOrDefault();
            return Ok(retVal);
        }

        [HttpGet]
        [Route("plans")]
        [ProducesResponseType(typeof(PaymentPlan[]), 200)]
        public async Task<IActionResult> GetPaymentPlanByIds([FromQuery] string[] ids)
        {
            var retVal = await _planService.GetByIdsAsync(ids);
            return Ok(retVal);
        }

        /// <summary>
        /// Gets plans by plenty ids 
        /// </summary>
        /// <param name="ids">Item ids</param>
        /// <returns></returns>
        [HttpPost]
        [Route("plans/plenty")]
        [ProducesResponseType(typeof(PaymentPlan[]), 200)]
        public async Task<IActionResult> GetPaymentPlansByPlentyIds([FromBody] string[] ids)
        {
            var retVal = await _planService.GetByIdsAsync(ids);
            return Ok(retVal);
        }


        [HttpPost]
        [Route("plans")]
        [ProducesResponseType(typeof(PaymentPlan), 200)]
        [Authorize(ModuleConstants.Security.Permissions.PlanManage)]
        public async Task<IActionResult> CreatePaymentPlan([FromBody] PaymentPlan plan)
        {
            await _planService.SavePlansAsync(new[] { plan });
            return Ok(plan);
        }



        [HttpPut]
        [Route("plans")]
        [ProducesResponseType(typeof(PaymentPlan), 200)]
        [Authorize(ModuleConstants.Security.Permissions.PlanManage)]
        public async Task<IActionResult> UpdatePaymentPlan([FromBody] PaymentPlan plan)
        {
            await _planService.SavePlansAsync(new[] { plan });
            return Ok(plan);
        }

        /// <summary>
        ///  Delete payment plans
        /// </summary>
        /// <param name="ids">plans' ids for delete</param>
        [HttpDelete]
        [Route("plans")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        [Authorize(ModuleConstants.Security.Permissions.PlanManage)]
        public async Task<IActionResult> DeletePlansByIds([FromQuery] string[] ids)
        {
            await _planService.DeleteAsync(ids);
            return NoContent();
        }

    }
}
