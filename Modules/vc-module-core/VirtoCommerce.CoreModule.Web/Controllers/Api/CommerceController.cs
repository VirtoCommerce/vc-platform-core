using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.CoreModule.Core;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.CoreModule.Core.Package;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Web.Controllers.Api
{
    [Route("api")]
    public class CommerceController : Controller
    {
        private readonly ICurrencyService _currencyService;
        private readonly IPackageTypesService _packageTypesService;
        private readonly ISeoDuplicatesDetector _seoDuplicateDetector;
        private readonly CompositeSeoBySlugResolver _seoBySlugResolverDecorator;
        public CommerceController(ICurrencyService currencyService, IPackageTypesService packageTypesService, ISeoDuplicatesDetector seoDuplicateDetector, CompositeSeoBySlugResolver seoBySlugResolverDecorator)
        {
            _currencyService = currencyService;
            _packageTypesService = packageTypesService;
            _seoDuplicateDetector = seoDuplicateDetector;
            _seoBySlugResolverDecorator = seoBySlugResolverDecorator;
        }


        /// <summary>
        /// Batch create or update seo infos
        /// </summary>
        /// <param name="seoInfos"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(void), 200)]
        [Route("seoinfos/batchupdate")]
        public Task<IActionResult> BatchUpdateSeoInfos(SeoInfo[] seoInfos)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [ProducesResponseType(typeof(SeoInfo[]), 200)]
        [Route("seoinfos/duplicates")]
        public async Task<IActionResult> GetSeoDuplicates(string objectId, string objectType)
        {
            var result = await _seoDuplicateDetector.DetectSeoDuplicatesAsync(new TenantIdentity(objectId, objectType));

            return Ok(result.ToArray());
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
            var retVal = await _seoBySlugResolverDecorator.FindSeoBySlugAsync(slug);
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
        [Authorize(ModuleConstants.Security.Permissions.CurrencyUpdate)]
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
        [Authorize(ModuleConstants.Security.Permissions.CurrencyCreate)]
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
        [Authorize(ModuleConstants.Security.Permissions.CurrencyDelete)]
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
        [Authorize(ModuleConstants.Security.Permissions.PackageTypeUpdate)]
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
        [Authorize(ModuleConstants.Security.Permissions.PackageTypeCreate)]
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
        [Authorize(ModuleConstants.Security.Permissions.PackageTypeDelete)]
        public async Task<IActionResult> DeletePackageTypes([FromQuery] string[] ids)
        {
            await _packageTypesService.DeletePackageTypesAsync(ids);
            return Ok();
        }
    }
}
