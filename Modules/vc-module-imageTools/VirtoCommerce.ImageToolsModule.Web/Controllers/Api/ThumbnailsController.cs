using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VirtoCommerce.ImageToolsModule.Core.Infrastructure;
using VirtoCommerce.ImageToolsModule.Core.Models;
using VirtoCommerce.ImageToolsModule.Core.ThumbnailGeneration;
using VirtoCommerce.ImageToolsModule.Web.Model;
using VirtoCommerce.Platform.Core.Settings;

using Security = VirtoCommerce.ImageToolsModule.Core.Security.SecurityConstants;

namespace VirtoCommerce.ImageToolsModule.Web.Controllers.Api
{
    /// <summary>
    /// Thumbnail of an image is an image with another size(resolution). 
    /// The thumbnails size is less of original size.
    /// Thumbnails are using in an interface, where it doesn't need high resolution.
    /// For example, in listings, short views.
    /// The Api controller allows to generate different thumbnails, get list of existed thumbnails
    /// </summary>
    [Route("api/image/thumbnails")]
    public class ThumbnailsController : Controller
    {
        private readonly IThumbnailGenerator _thumbnailsGenerator;
        private readonly ISettingsManager _settingsManager;

        /// <summary>
        /// Constructor
        /// </summary>
        public ThumbnailsController(IThumbnailGenerator thumbnailsGenerator, ISettingsManager settingsManager)
        {
            _thumbnailsGenerator = thumbnailsGenerator;
            _settingsManager = settingsManager;
        }

        /// <summary>
        /// Generate a number thumbnails of original image by given settings.
        /// </summary>
        /// <returns>True, if successfully done</returns>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(GenerateThumbnailsResponse), 200)]
        [Authorize(Security.Thumbnail.Read)]
        public async Task<IActionResult> GenerateAsync([FromBody]GenerateThumbnailsRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var setting = await _settingsManager.GetSettingByNameAsync("ImageTools.Thumbnails.Parameters");
            if (setting == null)
                return Ok(new GenerateThumbnailsResponse());

            var settings = setting.ArrayValues ?? new string[] { };
            var options = settings.Select(x => JsonConvert.DeserializeObject<ThumbnailOption>(x, new SettingJsonConverter())).ToList();
            var result = await _thumbnailsGenerator.GenerateThumbnailsAsync(request.ImageUrl, null, options, null);

            return Ok(new GenerateThumbnailsResponse());
        }
    }
}
