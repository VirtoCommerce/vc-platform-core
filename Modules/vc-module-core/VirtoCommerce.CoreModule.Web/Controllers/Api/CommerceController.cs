using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.CoreModule.Core.Model;
using VirtoCommerce.CoreModule.Core.Model.Tax;
using VirtoCommerce.CoreModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.CoreModule.Web.Controllers.Api
{
    [Route("api")]
    public class CommerceController : Controller
    {
        private readonly ISeoService _seoService;
        private readonly ICurrencyService _currencyService;
        private readonly IPackageTypesService _packageTypesService;
        private readonly IStoreService _storeService;
        private readonly ISeoDuplicatesDetector _seoDuplicateDetector;

        public CommerceController(ISeoService seoService, ICurrencyService currencyService, IPackageTypesService packageTypesService, IStoreService storeService, ISeoDuplicatesDetector seoDuplicateDetector)
        {
            _seoService = seoService;
            _currencyService = currencyService;
            _packageTypesService = packageTypesService;
            _storeService = storeService;
            _seoDuplicateDetector = seoDuplicateDetector;
        }


        /// <summary>
        /// Evaluate and return all tax rates for specified store and evaluation context 
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="evalContext"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(TaxRate[]), 200)]
        [Route("taxes/{storeId}/evaluate")]
        public async Task<IActionResult> EvaluateTaxes(string storeId, [FromBody]TaxEvaluationContext evalContext)
        {
            var retVal = new List<TaxRate>();
            var store = await _storeService.GetByIdAsync(storeId);
            if (store != null)
            {
                var activeTaxProvider = store.TaxProviders.FirstOrDefault(x => x.IsActive);
                if (activeTaxProvider != null)
                {
                    evalContext.StoreId = store.Id;
                    retVal.AddRange(activeTaxProvider.CalculateRates(evalContext));
                }
            }
            return Ok(retVal);
        }

        /// <summary>
        /// Batch create or update seo infos
        /// </summary>
        /// <param name="seoInfos"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(void), 200)]
        [Route("seoinfos/batchupdate")]
        public async Task<IActionResult> BatchUpdateSeoInfos(SeoInfo[] seoInfos)
        {
            await _seoService.SaveSeoInfosAsync(seoInfos);
            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(typeof(SeoInfo[]), 200)]
        [Route("seoinfos/duplicates")]
        public async Task<IActionResult> GetSeoDuplicates(string objectId, string objectType)
        {
            var seoDuplicates = await _seoService.GetAllSeoDuplicatesAsync();
            var retVal = _seoDuplicateDetector.DetectSeoDuplicates(objectType, objectId, seoDuplicates);
            return Ok(retVal.ToArray());
        }

        /// <summary>
        /// Find all SEO records for object by slug 
        /// </summary>
        /// <param name="slug">slug</param>
        [HttpGet]
        [ProducesResponseType(typeof(SeoInfo[]), 200)]
        [Route("seoinfos/{slug}")]
        public async Task<IActionResult> GetSeoInfoBySlug(string slug)
        {
            var retVal = await _seoService.GetSeoByKeywordAsync(slug);
            return Ok(retVal.ToArray());
        }

        /// <summary>
        /// Return all currencies registered in the system
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Currency[]), 200)]
        [Route("currencies")]
        public async Task<IActionResult> GetAllCurrencies()
        {
            var retVal = await _currencyService.GetAllCurrenciesAsync();
            return Ok(retVal.ToArray());
        }

        /// <summary>
        ///  Update a existing currency 
        /// </summary>
        /// <param name="currency">currency</param>
        [HttpPut]
        [ProducesResponseType(typeof(void), 200)]
        [Route("currencies")]
        //TODO
        //[CheckPermission(Permission = CommercePredefinedPermissions.CurrencyUpdate)]
        public async Task<IActionResult> UpdateCurrency([FromBody]Currency currency)
        {
            await _currencyService.SaveChangesAsync(new[] { currency });
            return Ok();
        }

        /// <summary>
        ///  Create new currency 
        /// </summary>
        /// <param name="currency">currency</param>
        [HttpPost]
        [ProducesResponseType(typeof(void), 200)]
        [Route("currencies")]
        //TODO
        //[CheckPermission(Permission = CommercePredefinedPermissions.CurrencyCreate)]
        public async Task<IActionResult> CreateCurrency([FromBody]Currency currency)
        {
            await _currencyService.SaveChangesAsync(new[] { currency });
            return Ok();
        }

        /// <summary>
        ///  Delete currencies 
        /// </summary>
        /// <param name="codes">currency codes</param>
        [HttpDelete]
        [ProducesResponseType(typeof(void), 200)]
        [Route("currencies")]
        //TODO
        //[CheckPermission(Permission = CommercePredefinedPermissions.CurrencyDelete)]
        public async Task<IActionResult> DeleteCurrencies([FromQuery] string[] codes)
        {
            await _currencyService.DeleteCurrenciesAsync(codes);
            return Ok();
        }


        /// <summary>
        /// Return all package types registered in the system
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PackageType[]), 200)]
        [Route("packageTypes")]
        public async Task<IActionResult> GetAllPackageTypes()
        {
            var retVal = await _packageTypesService.GetAllPackageTypesAsync();
            return Ok(retVal.ToArray());
        }

        /// <summary>
        ///  Update a existing package type 
        /// </summary>
        /// <param name="packageType">package type</param>
        [HttpPut]
        [ProducesResponseType(typeof(void), 200)]
        [Route("packageTypes")]
        //[CheckPermission(Permission = CommercePredefinedPermissions.PackageTypeUpdate)]
        public async Task<IActionResult> UpdatePackageType([FromBody]PackageType packageType)
        {
            await _packageTypesService.SaveChangesAsync(new[] { packageType });
            return Ok();
        }

        /// <summary>
        ///  Create new package type 
        /// </summary>
        /// <param name="packageType">package type</param>
        [HttpPost]
        [ProducesResponseType(typeof(void), 200)]
        [Route("packageTypes")]
        //[CheckPermission(Permission = CommercePredefinedPermissions.PackageTypeCreate)]
        public async Task<IActionResult> CreatePackageType(PackageType packageType)
        {
            await _packageTypesService.SaveChangesAsync(new[] { packageType });
            return Ok();
        }

        /// <summary>
        ///  Delete package types 
        /// </summary>
        /// <param name="ids">package type ids</param>
        [HttpDelete]
        [ProducesResponseType(typeof(void), 200)]
        [Route("packageTypes")]
        //[ProducesResponseType(Permission = CommercePredefinedPermissions.PackageTypeDelete)]
        public async Task<IActionResult> DeletePackageTypes([FromQuery] string[] ids)
        {
            await _packageTypesService.DeletePackageTypesAsync(ids);
            return Ok();
        }
    }
}
