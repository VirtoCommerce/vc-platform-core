using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.ImageToolsModule.Core.Models;
using VirtoCommerce.ImageToolsModule.Core.Services;
using Permission = VirtoCommerce.ImageToolsModule.Core.ModuleConstants.Security.Permissions;

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
        [Authorize(Permission.Create)]
        public async Task<ActionResult<ThumbnailOption>> CreateThumbnailOption([FromBody]ThumbnailOption option)
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
        [Authorize(Permission.Delete)]
        public async Task<ActionResult> DeleteThumbnailOption([FromQuery] string[] ids)
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
        [Authorize(Permission.Read)]
        public async Task<ActionResult<ThumbnailOption>> GetThumbnailOption([FromRoute]string id)
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
        [Authorize(Permission.Read)]
        public async Task<ActionResult<ThumbnailOptionSearchResult>> SearchThumbnailOption([FromBody]ThumbnailOptionSearchCriteria criteria)
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
        [Authorize(Permission.Update)]
        public async Task<ActionResult> UpdateThumbnailOption([FromBody]ThumbnailOption option)
        {
            await _thumbnailOptionService.SaveOrUpdateAsync(new[] { option });
            return Ok();
        }
    }
}
