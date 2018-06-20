using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.ImageToolsModule.Core.Models;
using VirtoCommerce.ImageToolsModule.Core.Services;

using Security = VirtoCommerce.ImageToolsModule.Core.Security.SecurityConstants;

namespace VirtoCommerce.ImageToolsModule.Web.Controllers.Api
{
    /// <summary>
    /// Thumbnails options controller
    /// </summary>
    [Route("api/image/thumbnails/options")]
    public class ThumbnailsOptionsController : Controller
    {
        private readonly IThumbnailOptionService _thumbnailOptionService;
        private readonly IThumbnailOptionSearchService _thumbnailOptionSearchService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="thumbnailOptionService"></param>
        /// <param name="thumbnailOptionSearchService"></param>
        public ThumbnailsOptionsController(IThumbnailOptionService thumbnailOptionService, IThumbnailOptionSearchService thumbnailOptionSearchService)
        {
            _thumbnailOptionService = thumbnailOptionService;
            _thumbnailOptionSearchService = thumbnailOptionSearchService;
        }

        /// <summary>
        /// Creates thumbnail option
        /// </summary>
        /// <param name="option">thumbnail option</param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(ThumbnailOption), 200)]
        [Authorize(Security.Thumbnail.Create)]
        public async Task<IActionResult> Create([FromBody]ThumbnailOption option)
        {
            await _thumbnailOptionService.SaveOrUpdateAsync(new[] { option });
            return Ok(option);
        }

        /// <summary>
        /// Remove thumbnail options by ids
        /// </summary>
        /// <param name="ids">options ids</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        [ProducesResponseType(typeof(void), 200)]
        [Authorize(Security.Thumbnail.Delete)]
        public async Task<IActionResult> Delete([FromQuery] string[] ids)
        {
            await _thumbnailOptionService.RemoveByIdsAsync(ids);
            return Ok();
        }

        /// <summary>
        /// Gets thumbnail options
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ThumbnailOption), 200)]
        [Authorize(Security.Thumbnail.Read)]
        public async Task<IActionResult> Get([FromQuery]string id)
        {
            var options = await _thumbnailOptionService.GetByIdsAsync(new[] { id });
            return Ok(options.FirstOrDefault());
        }

        /// <summary>
        /// Searches thumbnail options
        /// </summary>
        /// <param name="criteria">Search criteria</param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        [ProducesResponseType(typeof(ThumbnailOptionSearchResult), 200)]
        [Authorize(Security.Thumbnail.Read)]
        public async Task<IActionResult> Search([FromBody]ThumbnailOptionSearchCriteria criteria)
        {
            var result = await _thumbnailOptionSearchService.SearchAsync(criteria);
            return Ok(result);
        }

        /// <summary>
        /// Updates thumbnail options
        /// </summary>
        /// <param name="option">Thumbnail options</param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        [ProducesResponseType(typeof(void), 200)]
        [Authorize(Security.Thumbnail.Update)]
        public async Task<IActionResult> Update([FromBody]ThumbnailOption option)
        {
            await _thumbnailOptionService.SaveOrUpdateAsync(new[] { option });
            return Ok();
        }
    }
}
