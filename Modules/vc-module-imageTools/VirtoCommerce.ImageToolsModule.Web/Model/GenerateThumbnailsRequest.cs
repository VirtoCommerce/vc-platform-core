using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.ImageToolsModule.Web.Model
{
    /// <summary>
    /// Request to generate Thumbnails for a single image.
    /// </summary>
    public class GenerateThumbnailsRequest
    {
        /// <summary>
        /// Url of a platform blob storage image.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// True to replace all existed thumbnails with a new ones. False to generate missed only.
        /// </summary>
        public bool IsRegenerateAll { get; set; }
    }
}
