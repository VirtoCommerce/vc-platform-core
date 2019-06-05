using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.TaxModule.Core.Model;
using VirtoCommerce.TaxModule.Core.Model.Search;
using VirtoCommerce.TaxModule.Core.Services;

namespace VirtoCommerce.TaxModule.Web.Controllers.Api
{
    [Route("api/taxes")]
    public class TaxModuleController : Controller
    {
        private readonly ITaxProviderSearchService _taxProviderSearchService;
        private readonly ITaxProviderService _taxProviderService;
        public TaxModuleController(ITaxProviderSearchService taxProviderSearchService, ITaxProviderService taxProviderService)
        {
            _taxProviderSearchService = taxProviderSearchService;
            _taxProviderService = taxProviderService;
        }

        [HttpPost]
        [Route("search")]
        public async Task<ActionResult<TaxProviderSearchResult>> SearchTaxProviders([FromBody]TaxProviderSearchCriteria criteria)
        {
            var result = await _taxProviderSearchService.SearchTaxProvidersAsync(criteria);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<TaxProviderSearchResult>> GetTaxProviderById(string id)
        {
            var result = await _taxProviderService.GetByIdAsync(id, null);
            return Ok(result);
        }

        [HttpPut]
        [Route("")]
        public async Task<ActionResult<TaxProvider>> UpdateTaxProvider([FromBody]TaxProvider taxProvider)
        {
            await _taxProviderService.SaveChangesAsync(new[] { taxProvider });
            return Ok(taxProvider);
        }

        /// <summary>
        /// Evaluate and return all tax rates for specified store and evaluation context 
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="evalContext"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{storeId}/evaluate")]
        public async Task<ActionResult<TaxRate[]>> EvaluateTaxes(string storeId, [FromBody]TaxEvaluationContext evalContext)
        {
            var result = new List<TaxRate>();
            var storeTaxProviders = await _taxProviderSearchService.SearchTaxProvidersAsync(new TaxProviderSearchCriteria { StoreId = storeId });

            var activeTaxProvider = storeTaxProviders.Results.FirstOrDefault(x => x.IsActive);
            if (activeTaxProvider != null)
            {
                evalContext.StoreId = storeId;
                result.AddRange(activeTaxProvider.CalculateRates(evalContext));
            }

            return Ok(result);
        }
    }
}
