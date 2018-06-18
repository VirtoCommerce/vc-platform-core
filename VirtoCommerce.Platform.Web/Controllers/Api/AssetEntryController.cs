using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.Platform.Web.Controllers.Api
{
    [Route("api/platform/assetentries")]
    public class AssetEntryController : Controller
    {
        private readonly IAssetEntryService _assetService;
        private readonly IAssetEntrySearchService _assetSearchService;

        public AssetEntryController(IAssetEntryService assetService, IAssetEntrySearchService assetSearchService)
        {
            _assetService = assetService;
            _assetSearchService = assetSearchService;
        }

        /// <summary>
        /// SearchAsync for AssetEntries by AssetEntrySearchCriteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        [ProducesResponseType(typeof(GenericSearchResult<AssetEntry>), 200)]
        [Authorize(PlatformConstants.Security.Permissions.AssetAccess)]
        public async Task<IActionResult> Search([FromBody]AssetEntrySearchCriteria criteria)
        {
            var result = await _assetSearchService.SearchAssetEntriesAsync(criteria);
            return Ok(result);
        }

        /// <summary>
        /// Get asset details by id
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(AssetEntry), 200)]
        [Authorize(PlatformConstants.Security.Permissions.AssetRead)]
        public async Task<IActionResult> Get([FromQuery]string id)
        {
            var retVal = await _assetService.GetByIdsAsync(new[] { id });
            if (retVal?.Any() == true)
            {
                return Ok(retVal.Single());
            }

            return NotFound();
        }

        /// <summary>
        /// Create / Update asset entry
        /// </summary>
        [HttpPut]
        [Route("")]
        [ProducesResponseType(typeof(void), 200)]
        [Authorize(PlatformConstants.Security.Permissions.AssetUpdate)]
        public async Task<IActionResult> Update([FromBody]AssetEntry item)
        {
            await _assetService.SaveChangesAsync(new[] { item });
            return Ok();
        }

        /// <summary>
        /// Delete asset entries by ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(typeof(void), 200)]
        [Route("")]
        [Authorize(PlatformConstants.Security.Permissions.AssetDelete)]
        public async Task<IActionResult> Delete([FromQuery] string[] ids)
        {
            await _assetService.DeleteAsync(ids);
            return Ok();
        }
    }
}
