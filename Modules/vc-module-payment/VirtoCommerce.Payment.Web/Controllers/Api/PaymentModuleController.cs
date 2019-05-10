using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Model.Search;
using VirtoCommerce.PaymentModule.Core.Services;

namespace VirtoCommerce.PaymentModule.Web.Controllers.Api
{
    [Route("api/payment")]
    public class PaymentModuleController : Controller
    {
        private readonly IPaymentMethodsSearchService _paymentMethodsSearchService;
        private readonly IPaymentMethodsService _paymentMethodsService;

        public PaymentModuleController(
            IPaymentMethodsSearchService paymentMethodsSearchService,
            IPaymentMethodsService paymentMethodsService
            )
        {
            _paymentMethodsSearchService = paymentMethodsSearchService;
            _paymentMethodsService = paymentMethodsService;
        }

        [HttpPost]
        [Route("search")]
        public async Task<ActionResult<PaymentMethodsSearchResult>> SearchPaymentMethods([FromBody]PaymentMethodsSearchCriteria criteria)
        {
            var result = await _paymentMethodsSearchService.SearchPaymentMethodsAsync(criteria);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<PaymentMethod>> GetPaymentMethodById(string id)
        {
            var result = await _paymentMethodsService.GetByIdAsync(id, null);
            return Ok(result);
        }

        [HttpPut]
        [Route("")]
        public async Task<ActionResult<PaymentMethod>> UpdatePaymentMethod([FromBody]PaymentMethod paymentMethod)
        {
            await _paymentMethodsService.SaveChangesAsync(new[] { paymentMethod });
            return Ok(paymentMethod);
        }
    }
}
