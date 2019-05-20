using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model.Search;
using VirtoCommerce.ShippingModule.Core.Services;

namespace VirtoCommerce.ShippingModule.Web.Controllers.Api
{
    [Route("api/shipping")]
    public class ShippingModuleController : Controller
    {
        private readonly IShippingMethodsSearchService _shippingMethodsSearchService;
        private readonly IShippingMethodsService _shippingMethodsService;

        public ShippingModuleController(
            IShippingMethodsSearchService shippingMethodsSearchService,
            IShippingMethodsService shippingMethodsService
            )
        {
            _shippingMethodsSearchService = shippingMethodsSearchService;
            _shippingMethodsService = shippingMethodsService;
        }

        [HttpPost("search")]
        public async Task<ActionResult<ShippingMethodsSearchResult>> SearchShippingMethods([FromBody]ShippingMethodsSearchCriteria criteria)
        {
            var result = await _shippingMethodsSearchService.SearchShippingMethodsAsync(criteria);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ShippingMethod>> GetShippingMethodById(string id)
        {
            var result = await _shippingMethodsService.GetByIdAsync(id, null);
            return Ok(result);
        }

        [HttpPut("")]
        public async Task<ActionResult<ShippingMethod>> UpdateShippingMethod([FromBody]ShippingMethod shippingMethod)
        {
            await _shippingMethodsService.SaveChangesAsync(new[] { shippingMethod });
            return Ok(shippingMethod);
        }
    }
}
